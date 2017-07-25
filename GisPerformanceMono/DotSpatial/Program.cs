using System.Collections.Generic;
using System.IO;

namespace DotSpatial
{
    internal class Program
    {
        private const int NumberOfRuns = 1; // redoing all requests and getting average.
        private const int IterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        public static void Main(string[] args)
        {
            var vectorFiles = new List<string>
            {
                Path.Combine("res", "points", "points.shp"),
                Path.Combine("res", "TM_WORLD_BORDERS", "TM_WORLD_BORDERS.shp"),
                Path.Combine("res", "BLMAdminBoundaries", "BLMAdminBoundaries.shp")
            };
            var vectorPerformance = new DotSpatialVectorPerformance(vectorFiles);
            vectorPerformance.Start(NumberOfRuns, IterationPerRun);
            
            var rasterFiles = new List<string>
            {
                Path.Combine("res", "lakes_raster_z0.tif"),
                Path.Combine("res", "TerrametricsStd_GeoTiff.tif"),
                Path.Combine("res", "waust_tmo_2011062_geo.tif")
            };
            var rasterPerformance = new DotSpatialRasterPerformance(rasterFiles);
            rasterPerformance.Start(NumberOfRuns, IterationPerRun);
        }
    }
}
