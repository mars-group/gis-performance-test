using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GisPerformanceDotnetCore.tools;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;

namespace GisPerformanceDotnetCore
{
    public class PerformanceBenchmark
    {
        private readonly List<string> _files;
        private readonly GisType _gisType;
        private readonly Random _rnd;
        private const string PostGisConnString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
        private const string GeoServerConnString = "http://192.168.64.3:30083/gis/";

        public PerformanceBenchmark(List<string> files, GisType gisType)
        {
            _files = files;
            _gisType = gisType;
            _rnd = new Random();
        }

        public void TestPerformance(int numberOfRuns, int iterationPerRun)
        {
            var timeElapsed = _files.ToDictionary(dataId => dataId, dataId => new double[iterationPerRun]);

            // Test performance
            for (var run = 0; run < numberOfRuns; run++)
            {
                Console.WriteLine("\nRun " + (run + 1) + " started!");
                var requests = 1;
                for (var iterration = 0; iterration < iterationPerRun; iterration++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    Console.WriteLine(requests + " request(s) started.");
                    foreach (var file in _files)
                    {
                        switch (_gisType)
                        {
                            case GisType.GeoServer:
                                Thread.Sleep(2000);
                                timeElapsed[file][iterration] += GetRandomGeoServerValue(file, requests);
                                break;
                            case GisType.MongoDb:
                                Thread.Sleep(2000);
                                timeElapsed[file][iterration] += GetRandomMongoDbValue(file, requests);
                                break;
                            case GisType.PostgisRaster:
                                timeElapsed[file][iterration] += GetRandomPostGisRasterValue(file, requests);
                                break;
                            case GisType.PostGisVector:
                                timeElapsed[file][iterration] += GetRandomPostGisVectorValue(file, requests);
                                break;
                            case GisType.EsriAscii:
                                timeElapsed[file][iterration] += GetRandomEsriAsciiValue(file, requests);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    requests *= 10;
                }
            }

            // Calculate average
            foreach (var file in _files)
            {
                var read = 1;
                Console.WriteLine("\n" + file + ":");
                for (var iterration = 0; iterration < iterationPerRun; iterration++)
                {
                    Console.WriteLine(read + " reads took: " + timeElapsed[file][iterration] / numberOfRuns + " ms");
                    read *= 10;
                }
            }
        }

        private double GetRandomGeoServerValue(string file, int requests)
        {
            var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }) {BaseAddress = new Uri(GeoServerConnString)};

            //ToDo: generate random lng and lat

            var errorCount = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, requests, i =>
            {
                var response = client.GetAsync("value?dataId=" + file + "&lon=10.0&lat=10.0").Result;

                if (!response.IsSuccessStatusCode)
                {
                    errorCount++;
                }

                var result = response.Content.ReadAsStringAsync().Result;
//                Console.WriteLine("Response for [" + i + "]: " + result);
            });

            if (errorCount > 0)
            {
                Console.WriteLine("errorCount [" + file + "]" + errorCount);
            }

            return watch.ElapsedMilliseconds;
        }

        private double GetRandomMongoDbValue(string file, int requests)
        {
            var client = new MongoClient();
            var database = client.GetDatabase("gis");

            var collection = database.GetCollection<BsonDocument>(file);

            // ToDo: randomize query

            var filter = Builders<BsonDocument>.Filter.Eq("properties.NAME", "Germany");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, requests, index =>
            {
                var res = collection.Find(filter).ToList();
//                Console.WriteLine("res:" + res.ToJson());
            });

            return watch.ElapsedMilliseconds;
        }

        private double GetRandomPostGisVectorValue(string file, int requests)
        {
            var query = "SELECT ST_AsText(geom) as geom FROM public." + file;

            var gid = _rnd.Next(0, 246).ToString();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, requests, index =>
            {
                using (var conn = new NpgsqlConnection(PostGisConnString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(query + " WHERE gid = " + gid, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var value = reader.GetString(0);
//                                Console.WriteLine(value);
                            }
                        }
                    }
                }
            });
            return watch.ElapsedMilliseconds;
        }

        private double GetRandomPostGisRasterValue(string file, int requests)
        {
            var x = new int[requests];
            var y = new int[requests];

            Parallel.For(0, requests, index =>
            {
                x[index] = _rnd.Next(1, 246);
                y[index] = _rnd.Next(1, 246);
            });

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, requests, index =>
            {
                using (var conn = new NpgsqlConnection(PostGisConnString))
                {
                    conn.Open();

                    var query = "SELECT ST_Value(rast," + x[index] + ", " + y[index] + ") FROM public." + file;

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var value = reader.GetValue(0);
                                if (value != null && !(value is DBNull))
                                {
//                                    Console.WriteLine(value);
                                }
                                else
                                {
                                    Console.WriteLine("Kein Wert: X:" + x[index] + " Y:" + y[index]);
                                }
                            }
                        }
                    }
                }
            });
            return watch.ElapsedMilliseconds;
        }

        private double GetRandomEsriAsciiValue(string file, int requests)
        {
            var pixels = new Point[requests];
            var asc = new EsriAsciiParser(file);

            var width = asc.NumberOfColumns;
            var height = asc.NumberOfRows;

            Parallel.For((long) 0, requests,
                index => { pixels[index] = new Point(_rnd.Next(width), _rnd.Next(height)); });

            // Close the file and reopen after messurement has started

            var watch = System.Diagnostics.Stopwatch.StartNew();

            asc = new EsriAsciiParser(file);
            Parallel.ForEach(pixels, pixel =>
            {
                var value = asc.GetValue(pixel.X, pixel.Y);
//                    Console.WriteLine(value);
            });

            return watch.Elapsed.Milliseconds;
        }
    }
}