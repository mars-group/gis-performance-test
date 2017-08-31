using System;
using GisRasterLayer.util;
using Xunit;

namespace GisRasterLayerTest.util
{
    public class CoordinateTest
    {
        [Fact]
        public void Coordinate_xyZero_valid()
        {
            const int x = 0;
            const int y = 0;

            var coord = new Coordinate(x, y);

            Assert.Equal(coord.X, x);
            Assert.Equal(coord.Y, y);
        }

        [Fact]
        public void Coordinate_xZero_valid()
        {
            const int x = 0;
            const int y = 10;

            var coord = new Coordinate(x, y);

            Assert.Equal(coord.X, x);
            Assert.Equal(coord.Y, y);
        }
        
        [Fact]
        public void Coordinate_xNeg_ExceptionThrown()
        {
            const int x = -1;
            const int y = 0;

            Assert.Throws<ArgumentException>(() =>
                new Coordinate(x, y)
            );
        }
        
        [Fact]
        public void Coordinate_yZero_valid()
        {
            const int x = 10;
            const int y = 0;

            var coord = new Coordinate(x, y);

            Assert.Equal(coord.X, x);
            Assert.Equal(coord.Y, y);
        }

        [Fact]
        public void Coordinate_yNeg_ExceptionThrown()
        {
            const int x = 0;
            const int y = -1;

            Assert.Throws<ArgumentException>(() =>
                new Coordinate(x, y)
            );
        }
    }
}