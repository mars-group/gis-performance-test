using System.Collections.Generic;
using System.IO;

namespace GisPerformanceDotnetCore
{
    class Program
    {
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        private static readonly List<string> GeoServerDataIds = new List<string>
        {
            "190d36ad-e3c5-4245-9bff-b8b6463366ec"
        };

        private static readonly List<string> MongoDbVectorFiles = new List<string>
        {
            "TM_WORLD_BORDERS"
        };

        private static readonly List<string> PostGisRasterFiles = new List<string>
        {
            "lakes_raster_z0",
            "Terrametricsstd_geotiff",
            "waust_tmo_2011062_geo"
        };

        private static readonly List<string> PostGisVectorFiles = new List<string>
        {
            "points",
            "tm_world_borders",
            "blmadminboundaries"
        };

        private static readonly List<string> AsciiFiles = new List<string>
        {
            Path.Combine("res", "ascii_grid.asc"),
            Path.Combine("res", "knp_srtm90m.asc")
        };

        static void Main()
        {
            var geoServerPerformance = new PerformanceBenchmark(GeoServerDataIds, GisType.GeoServer);
            geoServerPerformance.TestPerformance(NumberOfRuns, IterationPerRun);

            var mongoGisPerformance = new PerformanceBenchmark(MongoDbVectorFiles, GisType.MongoDb);
            mongoGisPerformance.TestPerformance(NumberOfRuns, IterationPerRun);

            var postGisRasterPerformance = new PerformanceBenchmark(PostGisRasterFiles, GisType.PostgisRaster);
            postGisRasterPerformance.TestPerformance(NumberOfRuns, IterationPerRun);

            var postGisVectorPerformance = new PerformanceBenchmark(PostGisVectorFiles, GisType.PostGisVector);
            postGisVectorPerformance.TestPerformance(NumberOfRuns, IterationPerRun);

            var esriAsciiPerformance = new PerformanceBenchmark(AsciiFiles, GisType.EsriAscii);
            esriAsciiPerformance.TestPerformance(NumberOfRuns, IterationPerRun);
        }
    }
}