using System;
using System.Collections.Generic;
using System.IO;

namespace DotSpatial
{
    public class Program
    {
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        public static void Main(string[] args)
        {
            Console.WriteLine("DotSpatial performance:");
            
            var rasterFiles = new List<string>
            {
                Path.Combine("res", "lakes_raster_z0.tif"),
                Path.Combine("res", "TerrametricsStd_GeoTiff.tif")
            };
            var rasterPerformance = new DotSpatialPerformance(rasterFiles, GisType.Raster);
            rasterPerformance.TestPerformance(NumberOfRuns, IterationPerRun);

//            var vectorFiles = new List<string>
//            {
//                Path.Combine("res", "points", "points.shp"),
//                Path.Combine("res", "TM_WORLD_BORDERS", "TM_WORLD_BORDERS.shp"),
//                Path.Combine("res", "BLMAdminBoundaries", "BLMAdminBoundaries.shp")
//            };
//            var vectorPerformance = new DotSpatialPerformance(vectorFiles, GisType.Vector);
//            vectorPerformance.TestPerformance(NumberOfRuns, IterationPerRun);
        }
    }
}