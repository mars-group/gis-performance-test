using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace NetTopologySuiteDotnetCore
{
    class Program
    {
        private static readonly string ParkBorder = Path.Combine("res", "2016_kruger_national_park.json");
        private static readonly string AgentPath = Path.Combine("res", "path.json");

        static void Main()
        {
            Console.WriteLine("Reading: " + ParkBorder);

            if (!File.Exists(ParkBorder) || !File.Exists(AgentPath))
            {
                Console.WriteLine("File does not exist!");
                return;
            }

            var borderFile = File.OpenText(ParkBorder);
            var pathFile = File.OpenText(AgentPath);

            var borderGeoJson = borderFile.ReadToEnd();
            var pathGeoJson = pathFile.ReadToEnd();

            var reader = new GeoJsonReader();
            var borderCollection = reader.Read<FeatureCollection>(borderGeoJson);
            var pathCollection = reader.Read<FeatureCollection>(pathGeoJson);

            var border = borderCollection.Features[0].Geometry;
            var path1 = pathCollection.Features[0].Geometry;
            var path2 = pathCollection.Features[1].Geometry;

            Console.WriteLine(path1.Intersects(border));
            Console.WriteLine(path2.Intersects(border));
        }

//        private double GetRandomVectorValue(string file, int requests)
//        {
//            var featureIds = new int[requests];
//            var dataReader = new ShapefileDataReader(file, new GeometryFactory());
//
//            var numberOfFeatures = dataReader.RecordCount;
//            var rnd = new Random();
//            Parallel.For(0, requests, index => { featureIds[index] = rnd.Next(numberOfFeatures); });
//
//            // Close the file and reopen after messurement has started
//            dataReader.Close();
//            dataReader.Dispose();
//
//            var watch = System.Diagnostics.Stopwatch.StartNew();
//
//            var reader = new ShapefileReader(file);
//            var allFeatures = reader.ReadAll();
//
//            Parallel.ForEach(featureIds, id =>
//            {
//                var feature = allFeatures[id];
////                var result = feature;
////                Console.WriteLine(result);
//            });
//
//            return watch.Elapsed.Milliseconds;
//        }
    }
}