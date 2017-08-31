using System;

namespace GisRasterLayer.util
{
    public class Coordinate
    {
        public int X { get; }
        public int Y { get; }

        public Coordinate(int x, int y)
        {
            if (x < 0)
            {
                throw new ArgumentException("X can't be negative.");
            }
            
            if (y < 0)
            {
                throw new ArgumentException("Y can't be negative.");
            }
            
            X = x;
            Y = y;
        }
    }
}