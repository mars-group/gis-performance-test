using System;
using System.IO;
using System.Linq;

namespace GisRasterLayer.util
{
    public class AsciiGridParser
    {
        public int NumberOfColumns { get; set; }

        public int NumberOfRows { get; set; }

        public double XLowerLeftCorner { get; set; }

        public double YLowerLeftCorner { get; set; }

        public double CellSize { get; set; }

        public int NoDataValue { get; set; }

        public double[,] Data { get; set; }


        /// <summary>
        /// Parses an Esri Ascii Grid file into RAM and and allows querying.
        /// This was created since DotSpatial is not .NET Core compatible
        /// </summary>
        /// <param name="path">Path to the .asc file.</param>
        public AsciiGridParser(string path)
        {
            using (var file = File.OpenText(path))
            {
                ParseMetadata(file);
                ParseData(file);
            }
        }

        /// <summary>
        /// Retrieves a pixel value at a position.
        /// </summary>
        /// <param name="coord">The x and y coordinate of the pixel.</param>
        /// <returns>The value of the pixel.</returns>
        public double GetValue(Coordinate coord)
        {
            if (coord.X < 0 || coord.X >= NumberOfColumns || coord.Y < 0 || coord.Y >= NumberOfRows)
            {
                return NoDataValue;
            }

            return Data[coord.Y, coord.X];
        }

        /// <summary>
        /// Retrieves a pixel value at a position from GPS.
        /// </summary>
        /// <param name="gps">A Gps position for the desired pixel.</param>
        /// <returns>The value of the pixel.</returns>
        public double GetValueFromGps(Gps gps)
        {
            if (gps.Longitude < XLowerLeftCorner || gps.Longitude > XLowerLeftCorner + NumberOfColumns * CellSize ||
                gps.Latitude < YLowerLeftCorner || gps.Latitude > YLowerLeftCorner + NumberOfRows * CellSize)
            {
                return NoDataValue;
            }

            var coordinate = GetFieldFromGps(gps);

            return GetValue(coordinate);
        }

        /// <summary>
        /// Retrieves the Metadata of the file.
        /// </summary>
        /// <returns>Metadata as string.</returns>
        public string GetMetadata()
        {
            return "ncols " + NumberOfColumns +
                   "\nnrows " + NumberOfRows +
                   "\nxllcorner " + XLowerLeftCorner +
                   "\nyllcorner " + YLowerLeftCorner +
                   "\ncellsize " + CellSize +
                   "\nnodata_value " + NoDataValue;
        }

        /// <summary>
        /// Retrieves the data of the file.
        /// </summary>
        /// <returns>Data as a string.</returns>
        public string GetData()
        {
            var str = "";
            for (var row = 0; row < NumberOfRows; row++)
            {
                for (var column = 0; column < NumberOfColumns; column++)
                {
                    str += GetValue(new Coordinate(column, row)) + " ";
                }
                str = str.Trim();

                if (row < NumberOfRows - 1)
                {
                    str += "\n";
                }
            }

            return str;
        }

        /// <summary>
        /// Retrieves Metadata & Data of the file.
        /// </summary>
        /// <returns>Matadata & Data as string.</returns>
        public override string ToString()
        {
            return GetMetadata() + "\n" + GetData();
        }

        private void ParseMetadata(TextReader file)
        {
            for (var metadataRow = 0; metadataRow < 6; metadataRow++)
            {
                var line = file.ReadLine().Trim().ToLower();

                if (line.StartsWith("ncols"))
                {
                    NumberOfColumns = int.Parse(line.Substring("ncols".Length));
                }
                else if (line.StartsWith("nrows"))
                {
                    NumberOfRows = int.Parse(line.Substring("nrows".Length));
                }
                else if (line.StartsWith("xllcorner"))
                {
                    XLowerLeftCorner = double.Parse(line.Substring("xllcorner".Length));
                }
                else if (line.StartsWith("yllcorner"))
                {
                    YLowerLeftCorner = double.Parse(line.Substring("yllcorner".Length));
                }
                else if (line.StartsWith("cellsize"))
                {
                    CellSize = double.Parse(line.Substring("cellsize".Length));
                }
                else if (line.StartsWith("nodata_value"))
                {
                    NoDataValue = int.Parse(line.Substring("nodata_value".Length));
                }
                else
                {
                    throw new FormatException("No valid metadata field in: " + line);
                }
            }
        }

        private void ParseData(TextReader file)
        {
            if (NumberOfColumns < 1)
            {
                ParseMetadata(file);
            }

            Data = new double[NumberOfRows, NumberOfColumns];

            for (var row = 0; row < NumberOfRows; row++)
            {
                var line = file.ReadLine()
                    .Trim()
                    .Split(' ')
                    .Select(double.Parse).ToArray();

                for (var column = 0; column < NumberOfColumns; column++)
                {
                    Data[row, column] = line[column];
                }
            }
        }

        private Coordinate GetFieldFromGps(Gps gps)
        {
            var x = (int) ((gps.Longitude - XLowerLeftCorner) / CellSize);
            var y = (int) ((gps.Latitude - YLowerLeftCorner) / CellSize);

            return new Coordinate(x, y);
        }
    }
}