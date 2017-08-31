using System;
using System.IO;

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