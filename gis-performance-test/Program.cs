using GeoServer;

namespace gis_performance_test
{
    internal class Program
    {
        private static void Main()
        {
            var geoServerPerformance = new GeoServerPerformance();
            geoServerPerformance.GeoserverTest();
        }
    }
}