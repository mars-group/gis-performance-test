using System.IO;
using GisVectorLayer.util;
using Xunit;

namespace GisVectorLayerTest
{
    public class GisVectorLayerTest
    {
        private static readonly string ParkBorder = Path.Combine("res", "2016_kruger_national_park.json");
        private static readonly string AgentPaths = Path.Combine("res", "paths.json");

        [Fact]
        public void GisVectorLayerTest_valid()
        {
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.Equal(2, agentPaths.Features.Count);
        }

        [Fact]
        public void GisVectorLayerTest_FileDoesNotExist_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() =>
                new GisVectorLayer.GisVectorLayer("NonExistingFile.json")
            );
        }

        [Fact]
        public void GisVectorLayerTest_Intersects_true()
        {
            var parkBorder = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.True(parkBorder.Intersects(agentPaths.Features[1]));
        }

        [Fact]
        public void GisVectorLayerTest_Intersects_false()
        {
            var parkBorder = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.False(parkBorder.Intersects(agentPaths.Features[0]));
        }

        [Fact]
        public void GisVectorLayerTest_IsInside_false()
        {
            var parkBorder = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.False(agentPaths.IsInside(parkBorder.Features[0]));
        }

        [Fact]
        public void GisVectorLayerTest_Overlaps_false()
        {
            var parkBorder = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.False(parkBorder.Overlaps(agentPaths.Features[0]));
        }

        [Fact]
        public void GisVectorLayerTest_DistanceEqual_value()
        {
            var parkBorder = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.Equal(0, VectorGisUtils.Distance(parkBorder.Features[0], agentPaths.Features[0]));
        }

        [Fact]
        public void GisVectorLayerTest_DistanceNotEqual_value()
        {
            var parkBorder = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPaths = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.NotEqual(1.0, VectorGisUtils.Distance(parkBorder.Features[0], agentPaths.Features[0]));
        }
    }
}