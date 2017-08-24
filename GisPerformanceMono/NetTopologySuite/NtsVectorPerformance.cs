using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace NetTopologySuite
{
    public class NtsVectorPerformance
    {
        private readonly List<string> _files;
        private readonly Random _rnd;

        public NtsVectorPerformance(List<string> files)
        {
            _files = files;
            _rnd = new Random();
        }

        public void TestPerformance(int initialNumberOfRuns, int numberOfRuns, int iterationPerRun)
        {
            var timeElapsed = _files.ToDictionary(dataId => dataId, dataId => new double[iterationPerRun]);

            // Test performance
            for (var run = 0; run < numberOfRuns; run++)
            {
                Console.WriteLine("\nRun " + (run + 1) + " started!");
                var requests = initialNumberOfRuns;
                for (var iterration = 0; iterration < iterationPerRun; iterration++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    Console.WriteLine(requests + " request(s) started.");
                    foreach (var file in _files)
                    {
                        timeElapsed[file][iterration] += GetRandomVectorValue(file, requests);
                    }
                    requests *= 10;
                }
            }

            // Calculate average
            foreach (var file in _files)
            {
                var read = initialNumberOfRuns;
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
            var featureIds = new int[requests];
            var dataReader = new ShapefileDataReader(file, new GeometryFactory());

            var numberOfFeatures = dataReader.RecordCount;
            Parallel.For(0, requests, index => { featureIds[index] = _rnd.Next(numberOfFeatures); });

            // Close the file and reopen after messurement has started
            dataReader.Close();
            dataReader.Dispose();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var reader = new ShapefileReader(file);
            var allFeatures = reader.ReadAll();

            Parallel.ForEach(featureIds, id =>
            {
                var feature = allFeatures[id];
//                var result = feature;
//                Console.WriteLine(result);
            });

            return watch.Elapsed.Milliseconds;
        }
    }
}