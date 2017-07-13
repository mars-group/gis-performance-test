namespace GisPerformanceDotnetCore
{
    class Program
    {
        static void Main()
        {
            var geoServerPerformance = new GeoServerPerformance();
            geoServerPerformance.Start();

            var postGisPerformance = new PostGisPerformance();
            postGisPerformance.Start();

            var mongoGisPerformance = new MongoGisPerformance();
            mongoGisPerformance.Start();
        }
    }
}