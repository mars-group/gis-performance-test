using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace DotNetTopologySuite
{
    public class NtsVectorPerformance
    {
        private readonly List<string> _files;
        private readonly Random _rnd;

        public NtsVectorPerformance(List<string> files)
        {
            _rnd = new Random();
            _files = files;
        }

        public void Start(int numberOfRuns, int iterationPerRun)
        {
            var timeElapsed = _files.ToDictionary(dataId => dataId, dataId => new double[iterationPerRun]);

            // call one file before performance messurement starts
            foreach (var file in _files)
            {
//                Thread.Sleep(100);
                GetRandomVectorValue(file, 1);
            }

            // test performance
            for (var run = 0; run < numberOfRuns; run++)
            {
                Console.WriteLine("\nRun " + (run + 1) + " started!");
                var requests = 1;
                for (var iterration = 0; iterration < iterationPerRun; iterration++)
                {
                    foreach (var file in _files)
                    {
//                        Thread.Sleep(2000);
                        Console.WriteLine(requests + " request(s) started: " + file);
                        timeElapsed[file][iterration] += GetRandomVectorValue(file, requests);
                    }
                    requests *= 10;
                }
            }

            // calculate average
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

        private double GetRandomVectorValue(string file, int requests)
        {
            var reader = new ShapefileReader(file);
            var dataReader = new ShapefileDataReader(file, new GeometryFactory());

            var ids = new int[requests];

            var numberOfFeatures = dataReader.RecordCount;

            Parallel.For(0, requests, index => { ids[index] = _rnd.Next(numberOfFeatures); });

            var allFeatures = reader.ReadAll();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var id in ids)
            {
                var feature = allFeatures.Geometries[id];
//                Console.WriteLine(feature);
            }

            var elapsedTime = watch.Elapsed.Ticks;

            return (double) elapsedTime / TimeSpan.TicksPerMillisecond;
        }
    }
}