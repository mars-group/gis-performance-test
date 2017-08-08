using System;

namespace GisPerformanceDotnetCore.tools
{
    public class Coordinates
    {
        public double Longitude { get; }
        public double Latitude { get; }

        public Coordinates(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double DistanceTo(Coordinates targetCoordinates)
        {
            var theta = Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;
            var baseRad = Math.PI * Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;

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