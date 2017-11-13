using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace GisVectorLayer
{
    public class GisVectorLayer
    {
        private readonly FeatureCollection _featureCollection;
        public Collection<IFeature> Features => _featureCollection.Features;

        public GisVectorLayer(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename + " does not exist!");
            }

            var file = File.OpenText(filename);
            var geoJson = file.ReadToEnd();

            var reader = new GeoJsonReader();
            _featureCollection = reader.Read<FeatureCollection>(geoJson);

            if (Features.Count < 1)
            {
                throw new FileLoadException("No Features were found while parsing the GIS file.");
            }
        }

        public bool Intersects(IFeature feature)
        {
            Console.WriteLine("type: " + _featureCollection.Type);
            return Features.Any(f => f.Geometry.Intersects(feature.Geometry));
        }

        public bool IsInside(IFeature feature)
        {
            return Features.Any(f => f.Geometry.Contains(feature.Geometry));
        }

        public bool Overlaps(IFeature feature)
        {
            return Features.Any(f => f.Geometry.Overlaps(feature.Geometry));
        }
    }
}