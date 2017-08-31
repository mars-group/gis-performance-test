using System;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace NetTopologySuiteDotnetCore
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
            
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