﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotSpatial.Data;

namespace DotSpatial
{
    public class DotSpatialVectorPerformance
    {
        private readonly List<string> _files;
        private readonly Random _rnd;

        public DotSpatialVectorPerformance(List<string> files)
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
            var sf = Shapefile.OpenFile(file);

            var ids = new int[requests];

            var numberOfFeatures = sf.Features.Count;

            Parallel.For(0, requests, index => { ids[index] = _rnd.Next(numberOfFeatures); });

            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var id in ids)
            {
                var feature = sf.Features[id];
                var geometry = feature.Geometry;
//                Console.WriteLine(geometry);
            }

            var elapsedTime = watch.Elapsed.Ticks;

            sf.Close();
            sf.Dispose();

            return (double) elapsedTime / TimeSpan.TicksPerMillisecond;
        }
    }
}