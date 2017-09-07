using System.IO;
using GisPerformanceDotnetCore.util;
using Xunit;

namespace GisRasterLayerTest
{
    public class AsciiGridParserTest
    {
        private static readonly string ExistingFile = Path.Combine("res", "ascii_grid.asc");
        private static readonly string NonExistingFile = Path.Combine("test.asc");

        [Fact]
        public void AsciiGridParser_valid()
        {
            var parser = new AsciiGridParser(ExistingFile);

            Assert.NotNull(parser);

            Assert.Equal(3, parser.NumberOfColumns);
            Assert.Equal(2, parser.NumberOfRows);
            Assert.Equal(0, parser.XLowerLeftCorner);
            Assert.Equal(0, parser.YLowerLeftCorner);
            Assert.Equal(10, parser.CellSize);
            Assert.Equal(-9999, parser.NoDataValue);

            var data = new double[,]
            {
                {1, -9999, 3},
                {4, 5, 6}
            };
            Assert.Equal(data, parser.Data);
        }

        [Fact]
        public void AsciiGridParser_FileDoesNotExist_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() =>
                new AsciiGridParser(NonExistingFile)
            );
        }

        [Fact]
        public void getValue_CoordinateInsideRaster_value()
        {
            var parser = new AsciiGridParser(ExistingFile);
            var parserCellSize = parser.CellSize;

            Assert.Equal(1, parser.GetValue(new Coordinate(0, 0)));
        }

        [Fact]
        public void getValue_CoordinateOutsideRaster_NoDataValue()
        {
            var parser = new AsciiGridParser(ExistingFile);

            Assert.Equal(parser.NoDataValue, parser.GetValue(new Coordinate(3, 0)));
        }

        [Fact]
        public void GetValueFromGps_GpsInsideRaster_value()
        {
            var parser = new AsciiGridParser(ExistingFile);

            Assert.Equal(1, parser.GetValueFromGps(new Gps(0.0, 0.0)));
        }

        [Fact]
        public void GetValueFromGps_GpsOutsideRaster_NoDataValue()
        {
            var parser = new AsciiGridParser(ExistingFile);

            Assert.Equal(parser.NoDataValue, parser.GetValueFromGps(new Gps(31.0, 0.0)));
        }

        [Fact]
        public void GetMetadata_MetadataEqual_value()
        {
            var parser = new AsciiGridParser(ExistingFile);

            const string metadata = "ncols 3\n" +
                                    "nrows 2\n" +
                                    "xllcorner 0\n" +
                                    "yllcorner 0\n" +
                                    "cellsize 10\n" +
                                    "nodata_value -9999";

            Assert.Equal(metadata, parser.GetMetadata());
        }

        [Fact]
        public void GetDataEqual_value()
        {
            var parser = new AsciiGridParser(ExistingFile);

            const string data = "1 -9999 3\n" +
                                "4 5 6";

            Assert.Equal(data, parser.GetData());
        }
    }
}