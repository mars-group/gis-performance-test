using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotSpatial.Data;
using NetTopologySuite.Noding;

namespace GisPerformanceMono
{
    public class DotSpatialPerformance
    {
        private readonly List<string> _files;
        private readonly Random _rnd;

        public DotSpatialPerformance(List<string> files)
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
                Thread.Sleep(100);
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
                        Thread.Sleep(2000);
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
            var sf = Shapefile.OpenFile(file);

            var ids = new int[requests];

            var numberOfFeatures = sf.Features.Count;

            Parallel.For(0, requests - 1, index => { ids[index] = _rnd.Next(numberOfFeatures); });

            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            for (var index = 0; index < requests; index++)
            {
                var feature = sf.Features[ids[index]];
                var geometry = feature.Geometry;
//                Console.WriteLine(geometry);
            }

//            // parallel (slower)
//            Parallel.ForEach(ids, id => {
//                var feature = sf.Features[id];
//                var geometry = feature.Geometry;
////                Console.WriteLine(geometry);
//            });

            var elapsedTime = watch.Elapsed.Ticks;

            sf.Close();

            return (double) elapsedTime / TimeSpan.TicksPerMillisecond;
        }
    }
}
