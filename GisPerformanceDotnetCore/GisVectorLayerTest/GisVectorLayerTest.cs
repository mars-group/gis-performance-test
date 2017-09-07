using System.IO;
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
            var parkBorderLayer = new GisVectorLayer.GisVectorLayer(ParkBorder);
            var agentPathsLayer = new GisVectorLayer.GisVectorLayer(AgentPaths);

            Assert.False(parkBorderLayer.Intersects(agentPathsLayer.Features[0].Geometry));
            Assert.True(parkBorderLayer.Intersects(agentPathsLayer.Features[1].Geometry));
        }
    }
}
