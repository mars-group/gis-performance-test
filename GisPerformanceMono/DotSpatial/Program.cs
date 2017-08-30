using System;
using System.Collections.Generic;
using System.IO;

namespace DotSpatial
{
    public class Program
    {
        private const int InitialNumberOfRuns = 100; // redoing all requests and getting average.
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 3; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        public static void Main(string[] args)
        {
            Console.WriteLine("DotSpatial performance:");
            
            var rasterFiles = new List<string>
            {
                Path.Combine("res", "lakes_raster_z0.tif"),
                Path.Combine("res", "TerrametricsStd_GeoTiff.tif"),
                Path.Combine("res", "waust_tmo_2011062_geo.tif") // Not in repo due to size
            };
            var rasterPerformance = new DotSpatialPerformance(rasterFiles, GisType.Raster);
            rasterPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);

            var vectorFiles = new List<string>
            {
                Path.Combine("res", "points", "points.shp"),
                Path.Combine("res", "TM_WORLD_BORDERS", "TM_WORLD_BORDERS.shp"),
                Path.Combine("res", "BLMAdminBoundaries", "BLMAdminBoundaries.shp") // Not in repo due to size
            };
            var vectorPerformance = new DotSpatialPerformance(vectorFiles, GisType.Vector);
            vectorPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);
        }
    }
}