using System;

namespace GisRasterLayer.util
{
    public class Coordinate
    {
        public int X;
        public int Y;

        public Coordinate(int x, int y)
        {
            if (x < 0)
            {
                throw new ArgumentException("x can't be negative.");
            }
            
            if (y < 0)
            {
                throw new ArgumentException("y can't be negative.");
            }
            
            X = x;
            Y = y;
        }
    }
}