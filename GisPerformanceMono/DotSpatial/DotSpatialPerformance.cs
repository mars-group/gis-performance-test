using System;
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
                        timeElapsed[file][iterration] += GetRandomValue(file, requests);
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
                var width = raster.Width;
                var height = raster.Height;
                Parallel.For(0, requests,
                    index => { pixels[index] = new Point(_rnd.Next(width), _rnd.Next(height)); });
            }

            // Close the file and reopen after messurement has started

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // ToDo: Make this line work to allow reading GeoTiff with Raster.Open().
            // blocked by: https://github.com/DotSpatial/DotSpatial/issues/901
//            GdalConfiguration.ConfigureGdal();
            
//            using (var raster = Raster.Open(file))
            using (var rasterFile = ImageData.Open(file))
            {
                var raster = rasterFile;
                Parallel.ForEach(pixels, pixel =>
                {
                    var color = raster.GetColor(pixel.Y, pixel.X);
//                    Console.WriteLine(color);
                });
            }

            return watch.Elapsed.Milliseconds;
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

            using (var shapefile = Shapefile.OpenFile(file))
            {
                var shp = shapefile;
                Parallel.ForEach(featureIds, id =>
                {
                    var feature = shp.GetFeature(id);
                    var geometry = feature.Geometry;
//                    Console.WriteLine(geometry);
                });
            }

            return watch.Elapsed.Milliseconds;
        }
    }
}