using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DotSpatial.Data;

namespace DotSpatial
{
    public class DotSpatialRasterPerformance
    {
        private readonly List<string> _files;
        private readonly Random _rnd;

        public DotSpatialRasterPerformance(List<string> files)
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
                GetRandomRasterValue(file, 1);
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
                        timeElapsed[file][iterration] += GetRandomRasterValue(file, requests);
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

        private double GetRandomRasterValue(string file, int requests)
        {
            var raster = ImageData.Open(file);

            var pixels = new Point[requests];
            Parallel.For(0, requests,
                index => { pixels[index] = new Point(_rnd.Next(raster.Width), _rnd.Next(raster.Height)); });

            var bytes = raster.Values;
            var image = new Argb[raster.Width, raster.Height];

            // Convert byte array to 2d point array
            var column = 0;
            var row = 0;
            for (long i = 0; i < bytes.Length; i += 4)
            {
                image[column, row] = new Argb(bytes[i + 3], bytes[i], bytes[i + 1], bytes[i + 2]);
                column++;
                if (column < raster.Width) continue;
                column = 0;
                row++;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var pixel in pixels)
            {
                var value = image[pixel.X, pixel.Y].ToColor();
//                Console.WriteLine(value);
            }

            var elapsedTime = watch.Elapsed.Ticks;

            raster.Close();

            return (double) elapsedTime / TimeSpan.TicksPerMillisecond;
        }
    }
}
