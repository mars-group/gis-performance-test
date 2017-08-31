using System;
using GisRasterLayer.util;
using Xunit;

namespace GisRasterLayerTest.util
{
    public class GpsTest
    {
        [Fact]
        public void Gps_lonLatCenter_valid()
        {
            const int longitude = 0;
            const int latitude = 0;

            var gps = new Gps(longitude, latitude);

            Assert.Equal(gps.Longitude, longitude);
            Assert.Equal(gps.Latitude, latitude);
        }

        [Fact]
        public void Gps_lonNeg180_valid()
        {
            const int longitude = -180;
            const int latitude = 0;

            var gps = new Gps(longitude, latitude);

            Assert.Equal(gps.Longitude, longitude);
            Assert.Equal(gps.Latitude, latitude);
        }
        
        [Fact]
        public void Gps_lonSmallerThanNeg180_ExceptionThrown()
        {
            const int longitude = -181;
            const int latitude = 0;

            Assert.Throws<ArgumentException>(() =>
                new Gps(longitude, latitude)
            );
        }
        
        [Fact]
        public void Gps_lon180_valid()
        {
            const int longitude = 180;
            const int latitude = 0;

            var gps = new Gps(longitude, latitude);

            Assert.Equal(gps.Longitude, longitude);
            Assert.Equal(gps.Latitude, latitude);
        }

        [Fact]
        public void Gps_lonBiggerThan180_ExceptionThrown()
        {
            const int longitude = 181;
            const int latitude = 0;

            Assert.Throws<ArgumentException>(() =>
                new Gps(longitude, latitude)
            );
        }
        
        [Fact]
        public void Gps_latNeg90_valid()
        {
            const int longitude = 0;
            const int latitude = -90;

            var gps = new Gps(longitude, latitude);

            Assert.Equal(gps.Longitude, longitude);
            Assert.Equal(gps.Latitude, latitude);
        }

        [Fact]
        public void Gps_latSmallerThanNeg90_ExceptionThrown()
        {
            const int longitude = 0;
            const int latitude = -91;

            Assert.Throws<ArgumentException>(() =>
                new Gps(longitude, latitude)
            );
        }
        
        [Fact]
        public void Gps_lat90_valid()
        {
            const int longitude = 0;
            const int latitude = 90;

            var gps = new Gps(longitude, latitude);

            Assert.Equal(gps.Longitude, longitude);
            Assert.Equal(gps.Latitude, latitude);
        }

        [Fact]
        public void Gps_latBiggerThan90_ExceptionThrown()
        {
            const int longitude = 0;
            const int latitude = 91;

            Assert.Throws<ArgumentException>(() =>
                new Gps(longitude, latitude)
            );
        }
    }
}