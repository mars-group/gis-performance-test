using System;
using System.IO;

namespace GisRasterLayer
{
    class Program
    {
        private static readonly string SmallFile = Path.Combine("res", "raster_small.asc");

        static void Main()
        {
            var parser = new AsciiGridParser(SmallFile);
            Console.WriteLine(parser.GetMetadata());
        }
    }
}