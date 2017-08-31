using System;

namespace GisPerformanceDotnetCore.util
{

    public class Gps
    {
        public double Longitude { get; }
        public double Latitude { get; }

        public Gps(double longitude, double latitude)
        {
            if (longitude < -180 || longitude > 180)
            {
                throw new ArgumentException("Longitude out of bounds!");
            }

            if (latitude < -90 || latitude > 90)
            {
                throw new ArgumentException("Latitude out of bounds!");
            }

            Longitude = longitude;
            Latitude = latitude;
        }

        public double DistanceTo(Gps targetGps)
        {
            var theta = Longitude - targetGps.Longitude;
            var thetaRad = Math.PI * theta / 180;
            var baseRad = Math.PI * Latitude / 180;
            var targetRad = Math.PI * targetGps.Latitude / 180;

            var dist = Math.Sin(baseRad) * Math.Sin(targetRad) +
                       Math.Cos(baseRad) * Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            const double kilometers = 1.609344;

            return kilometers * dist;
        }
    }
}