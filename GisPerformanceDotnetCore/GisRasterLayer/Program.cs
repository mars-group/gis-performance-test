using System;
using System.IO;
using GisRasterLayer.util;

namespace GisRasterLayer
{
    class Program
    {
        private static readonly string SmallFile = Path.Combine("res", "ascii_grid.asc");

        static void Main()
        {
            var parser = new AsciiGridParser(SmallFile);
            Console.WriteLine(parser.GetMetadata());
            Console.WriteLine(parser.GetData());
        }
    }
}