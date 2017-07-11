using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace main
{
    public class PostGisPerformance
    {
        private const string ConnString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)
        private readonly Random _rnd;

        private enum DataType
        {
            Raster,
            Vector
        }

        public PostGisPerformance()
        {
            _rnd = new Random();
        }

        public void Start()
        {
            var rasterFiles = new List<string>
            {
                "lakes_raster_z0",
                "Terrametricsstd_geotiff",
                "waust_tmo_2011062_geo"
            };

            var vectorFiles = new List<string>
            {
                "points",
                "tm_world_borders",
                "blmadminboundaries"
            };

            MessurePerformance(rasterFiles, DataType.Raster);
            MessurePerformance(vectorFiles, DataType.Vector);
        }

        private void MessurePerformance(IReadOnlyCollection<string> files, DataType dataType)
        {
            Console.WriteLine("//\n// " + dataType + ":\n//");

            var timeElapsed = files.ToDictionary(dataId => dataId, dataId => new long[IterationPerRun]);

            // call one file before performance messurement starts
            foreach (var file in files)
            {
                Thread.Sleep(100);
                if (dataType.Equals(DataType.Raster))
                {
                    GetRandomRasterValue(file, 1);
                }
                else
                {
                    GetRandomVectorValue(file, 1);
                }
            }

            // test performance
            for (var run = 0; run < NumberOfRuns; run++)
            {
                Console.WriteLine("\nRun " + (run + 1) + " started!");
                var requests = 1;
                for (var iterration = 0; iterration < IterationPerRun; iterration++)
                {
                    foreach (var file in files)
                    {
                        Thread.Sleep(2000);
                        if (dataType.Equals(DataType.Raster))
                        {
                            // skip too many requests on the big file to prevent DB crash.
                            if (file.Equals("waust_tmo_2011062_geo") && requests > 10)
                            {
                                continue;
                            }

                            Console.WriteLine(requests + " request(s) started: " + file);
                            timeElapsed[file][iterration] += GetRandomRasterValue(file, requests);
                        }
                        else
                        {
                            Console.WriteLine(requests + " request(s) started: " + file);
                            timeElapsed[file][iterration] += GetRandomVectorValue(file, requests);
                        }
                    }
                    requests *= 10;
                }
            }

            // calculate average
            foreach (var file in files)
            {
                var read = 1;
                Console.WriteLine("\n" + file + ":");
                for (var iterration = 0; iterration < IterationPerRun; iterration++)
                {
                    Console.WriteLine(read + " reads took: " + timeElapsed[file][iterration] / NumberOfRuns + " ms");
                    read *= 10;
                }
            }
        }

        private long GetRandomRasterValue(string file, int requests)
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
                using (var conn = new NpgsqlConnection(ConnString))
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

        private long GetRandomVectorValue(string file, int requests)
        {
            var query = "SELECT ST_AsText(geom) as geom FROM public." + file;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, requests, index =>
            {
                using (var conn = new NpgsqlConnection(ConnString))
                {
                    conn.Open();

                    var gid = _rnd.Next(0, 246).ToString();

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
    }
}