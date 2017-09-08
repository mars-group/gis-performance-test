using NetTopologySuite.Features;

namespace GisVectorLayer.util
{
    public static class VectorGisUtils
    {
        public static double Distance(IFeature featureA, IFeature featureB)
        {
            return featureA.Geometry.Distance(featureB.Geometry);
        }
    }
}