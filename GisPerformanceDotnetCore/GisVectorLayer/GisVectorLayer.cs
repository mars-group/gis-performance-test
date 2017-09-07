using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace GisVectorLayer
{
    public class GisVectorLayer
    {
        public readonly IFeature[] Features;

        public GisVectorLayer(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("File does not exist!");
                return;
            }

            var file = File.OpenText(filename);
            var geoJson = file.ReadToEnd();

            var reader = new GeoJsonReader();
            var featureCollection = reader.Read<FeatureCollection>(geoJson);

            Features = featureCollection.Features.ToArray();
        }

        public bool Intersects(IGeometry geometry)
        {
            return Features.Any(feature => feature.Geometry.Intersects(geometry));
        }
    }
}