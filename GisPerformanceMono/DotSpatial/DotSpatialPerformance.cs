﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DotSpatial.Data;

namespace DotSpatial
{
    public class DotSpatialPerformance
    {
        private readonly List<string> _files;
        private readonly GisType _gisType;
        private readonly Random _rnd;

        public DotSpatialPerformance(List<string> files, GisType gisType)
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
                    foreach (var file in _files)
                    {
                        Console.WriteLine(requests + " request(s) started: " + file);
                        timeElapsed[file][iterration] += GetRandomValue(file, requests);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
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

        private double GetRandomValue(string file, int requests)
        {
            return _gisType.Equals(GisType.Raster)
                ? GetRandomRasterValue(file, requests)
                : GetRandomVectorValue(file, requests);
        }

        private double GetRandomRasterValue(string file, int requests)
        {
            var pixels = new Point[requests];

            using (var raster = ImageData.Open(file))
            {
                Parallel.For(0, requests,
                    index => { pixels[index] = new Point(_rnd.Next(raster.Width), _rnd.Next(raster.Height)); });

                // Close the file and reopen after messurement has started
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            int elapsedTime;

            using (var raster = ImageData.Open(file))
            {
                Parallel.ForEach(pixels, pixel =>
                {
                    var color = raster.GetColor(pixel.Y, pixel.X);
//                    Console.WriteLine(color);
                });

                elapsedTime = watch.Elapsed.Milliseconds;
            }

            return elapsedTime;
        }

        private double GetRandomVectorValue(string file, int requests)
        {
            var featureIds = new int[requests];
            using (var shapefile = Shapefile.OpenFile(file))
            {
                var numberOfFeatures = shapefile.Features.Count;
                Parallel.For(0, requests, index => { featureIds[index] = _rnd.Next(numberOfFeatures); });
            }

            // Close the file and reopen after messurement has started

            var watch = System.Diagnostics.Stopwatch.StartNew();
            int elapsedTime;

            using (var shapefile = Shapefile.OpenFile(file))
            {
                Parallel.ForEach(featureIds, id =>
                {
                    var feature = shapefile.GetFeature(id);
                    var geometry = feature.Geometry;
//                    Console.WriteLine(geometry);
                });

                elapsedTime = watch.Elapsed.Milliseconds;
            }

            return elapsedTime;
        }
    }
}