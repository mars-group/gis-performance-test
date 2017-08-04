using System;
using System.IO;

namespace GisPerformanceDotnetCore
{
    class Program
    {
        private static readonly string File = Path.Combine("res", "ascii_grid.asc");

        static void Main()
        {
//            var geoServerPerformance = new GeoServerPerformance();
//            geoServerPerformance.Start();
//
//            var postGisPerformance = new PostGisPerformance();
//            postGisPerformance.Start();
//
//            var mongoGisPerformance = new MongoGisPerformance();
//            mongoGisPerformance.Start();

            var asc = new EsriAscii(File);
            Console.WriteLine(asc.GetMetadata());
        }
    }
}
