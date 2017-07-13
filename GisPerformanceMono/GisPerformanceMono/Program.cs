using System;
using System.IO;
using DotSpatial.Data;


namespace GisPerformanceMono
{
    internal class Program
    {
        private static readonly string Shape = Path.Combine("res", "TM_WORLD_BORDERS", "TM_WORLD_BORDERS.shp");

        public static void Main(string[] args)
        {
            ReadShp(Shape);
        }

        private static void ReadShp(string path)
        {
            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist!");
                return;
            }

            var sf = Shapefile.OpenFile(path);
//            sf.Reproject(DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984);

            var rnd = new Random();

            var featureId = rnd.Next(sf.Features.Count - 1);
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            var feature = sf.Features[featureId];

            var geometry = feature.Geometry;
//            Console.WriteLine(geometry);

            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
