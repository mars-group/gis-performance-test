using System;
using System.Collections.Generic;
using System.IO;
using GisPerformanceDotnetCore.util;

namespace GisPerformanceDotnetCore
{
    class Program
    {
        private const int InitialNumberOfRuns = 100; // redoing all requests and getting average.
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 2; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        private static readonly List<string> GeoServerDataIds = new List<string>
        {
//            "de1defdc-b3b3-4981-ae39-b92311212a5d",
//            "dd727485-87ae-4fbf-8eb4-499b9c3b62a3",
//            "cd48f7dd-012c-4117-a238-025ff8ad2847",
            
            "",
            "",
            ""
        };

        private static readonly List<string> MongoDbVectorFiles = new List<string>
        {
//            "points2", //warmup
            "points",
            "TM_WORLD_BORDERS",
            "BLMAdminBoundaries"
        };

        private static readonly List<string> PostGisRasterFiles = new List<string>
        {
            "lakes_raster_z02", //warmup
            "lakes_raster_z0",
            "Terrametricsstd_geotiff",
//            "waust_tmo_2011062_geo"
        };

        private static readonly List<string> PostGisVectorFiles = new List<string>
        {
            "points2", //warmup
            "points",
            "tm_world_borders",
            "blmadminboundaries"
        };

        private static readonly List<string> AsciiFiles = new List<string>
        {
            Path.Combine("res", "ascii_grid.asc"), //warmup
            Path.Combine("res", "raster_small.asc"),
            Path.Combine("res", "raster_mid.asc"),
//            Path.Combine("res", "raster_big.asc")
        };

        static void Main()
        {
//            var distance = new Coordinates(15.695585, 48.672309).DistanceTo(new Coordinates(16.389477, 48.237867));
//            Console.WriteLine(distance + "km");

            var geoServerPerformance = new PerformanceBenchmark(GeoServerDataIds, GisType.GeoServer);
//            geoServerPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);

            var mongoGisPerformance = new PerformanceBenchmark(MongoDbVectorFiles, GisType.MongoDb);
//            mongoGisPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);

            var postGisRasterPerformance = new PerformanceBenchmark(PostGisRasterFiles, GisType.PostgisRaster);
//            postGisRasterPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);

            var postGisVectorPerformance = new PerformanceBenchmark(PostGisVectorFiles, GisType.PostGisVector);
//            postGisVectorPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);

            var esriAsciiPerformance = new PerformanceBenchmark(AsciiFiles, GisType.EsriAscii);
            esriAsciiPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);
        }
    }
}