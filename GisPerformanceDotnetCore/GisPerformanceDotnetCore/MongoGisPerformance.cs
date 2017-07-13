using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GisPerformanceDotnetCore
{
    public class MongoGisPerformance
    {
        protected static IMongoClient Client;
        protected static IMongoDatabase Database;

        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        public void Start()
        {
            Client = new MongoClient();
            Database = Client.GetDatabase("gis");

            var vectorFiles = new List<string>
            {
                "TM_WORLD_BORDERS"
            };

            MessurePerformance(vectorFiles);
        }

        private static void MessurePerformance(IReadOnlyCollection<string> files)
        {
            var timeElapsed = files.ToDictionary(dataId => dataId, dataId => new long[IterationPerRun]);

            // call one file before performance messurement starts
            foreach (var file in files)
            {
                Thread.Sleep(100);
                GetRandomVectorValue(file, 1);
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
                        Console.WriteLine(requests + " request(s) started: " + file);
                        timeElapsed[file][iterration] += GetRandomVectorValue(file, requests);
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

        private static long GetRandomVectorValue(string document, int requests)
        {
            var collection = Database.GetCollection<BsonDocument>(document);
            var filter = Builders<BsonDocument>.Filter.Eq("properties.NAME", "Germany");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, requests, index =>
            {
                var res = collection.Find(filter).ToList();
//                Console.WriteLine("res:" + res.ToJson());
            });

            return watch.ElapsedMilliseconds;
        }
    }
}