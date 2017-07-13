using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GisPerformanceDotnetCore
{
    public class GeoServerPerformance
    {
        private static HttpClient _client;

        public void GeoserverTest()
        {
        }

        public void Start()
        {
            const int numberOfRuns = 2; // redoing all requests and getting average
            const int iterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

            var dataIds = new List<string>
            {
                "d0376004-ecd9-4e51-9bb6-342f0dc51a77", // tif big
                "b7e0ebee-af43-4520-b07a-fe9bdb09e3e6", // tif mid
                "edda5bfc-871d-46b6-8f2a-cc63141b2f24", // tif small
//                
                "6e3252ee-ba5a-4207-85da-ed44743d6d52", // shp big
                "68744b23-1f8c-46d4-9466-cd40d79cbc37", // shp mid
                "ebed97a6-bc47-43ac-8c0b-934d35a3620e" // shp small
            };

            var timeElapsed = dataIds.ToDictionary(dataId => dataId, dataId => new long[iterationPerRun]);

            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }) {BaseAddress = new Uri("http://192.168.64.3:30083/gis/")};

            // call one file before performance messurement starts
            foreach (var dataId in dataIds)
            {
                Thread.Sleep(100);
                GetGisValue(dataId, 1);
            }

            // test performance
            for (var run = 0; run < numberOfRuns; run++)
            {
                Console.WriteLine("\nRun " + (run + 1) + " started!");
                var requests = 1;
                for (var iterration = 0; iterration < iterationPerRun; iterration++)
                {
                    Console.WriteLine("Iterration with " + requests + " requests started!");
                    foreach (var dataId in dataIds)
                    {
                        Thread.Sleep(2000);
                        timeElapsed[dataId][iterration] += GetGisValue(dataId, requests);
                    }
                    requests *= 10;
                }
            }

            // calculate average
            foreach (var dataId in dataIds)
            {
                var read = 1;
                Console.WriteLine("\n" + dataId + ":");
                for (var iterration = 0; iterration < iterationPerRun; iterration++)
                {
                    Console.WriteLine(read + " reads took: " + timeElapsed[dataId][iterration] / numberOfRuns + " ms");
                    read *= 10;
                }
            }
        }

        private static long GetGisValue(string dataId, int n)
        {
            var errorCount = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, n, i =>
            {
                var response = _client.GetAsync("value?dataId=" + dataId + "&lon=10.0&lat=10.0").Result;
                //                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                {
                    errorCount++;
                }

                var result = response.Content.ReadAsStringAsync().Result;
//                Console.WriteLine("Response for [" + i + "]: " + result);
            });

            if (errorCount > 0)
            {
                Console.WriteLine("errorCount [" + dataId + "]" + errorCount);
            }

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}