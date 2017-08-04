using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite
{
    internal class Program
    {
        private const int NumberOfRuns = 3; // redoing all requests and getting average.
        private const int IterationPerRun = 4; // number of times to multiply iteration by 10 (eg. 1,10,100,1000 on 4)

        public static void Main(string[] args)
        {
            Console.WriteLine("NetTopologySuite performance:");
            
            var vectorFiles = new List<string>
            {
                Path.Combine("res", "points", "points.shp"),
                Path.Combine("res", "TM_WORLD_BORDERS", "TM_WORLD_BORDERS.shp")
            };
            var ntsPerformance = new NtsVectorPerformance(vectorFiles);
            ntsPerformance.TestPerformance(NumberOfRuns, IterationPerRun);
        }
    }
}
