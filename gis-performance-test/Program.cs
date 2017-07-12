namespace main
{
    internal class Program
    {
        private static void Main()
        {
//            var geoServerPerformance = new GeoServerPerformance();
//            geoServerPerformance.Start();
            
//            var postGisPerformance = new PostGisPerformance();
//            postGisPerformance.Start();

            var mongoGisPerformance = new MongoGisPerformance();
            mongoGisPerformance.Start();
        }
    }

}
