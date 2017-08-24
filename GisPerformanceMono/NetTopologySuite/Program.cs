using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite
{
    internal class Program
    {
        private const int InitialNumberOfRuns = 1000; // redoing all requests and getting average.
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 1; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        public static void Main(string[] args)
        {
            Console.WriteLine("NetTopologySuite performance:");
            
            var vectorFiles = new List<string>
            {
                Path.Combine("res", "points", "points.shp"),
                Path.Combine("res", "TM_WORLD_BORDERS", "TM_WORLD_BORDERS.shp"),
                Path.Combine("res", "BLMAdminBoundaries", "BLMAdminBoundaries.shp") // Not in repo due to size
            };
            var ntsPerformance = new NtsVectorPerformance(vectorFiles);
            ntsPerformance.TestPerformance(InitialNumberOfRuns, NumberOfRuns, IterationPerRun);
        }
    }
}
