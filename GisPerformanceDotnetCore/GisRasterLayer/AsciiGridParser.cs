using System;
using System.IO;
using System.Linq;
using GisRasterLayer.util;

namespace GisRasterLayer
{
    public class AsciiGridParser
    {
        private int NumberOfColumns { get; set; }

        private int NumberOfRows { get; set; }

        private double XLowerLeftCorner { get; set; }

        private double YLowerLeftCorner { get; set; }

        private double CellSize { get; set; }

        private int NoDataValue { get; set; }

        private double[][] Data { get; set; }


        public AsciiGridParser(string path)
        {
            using (var file = File.OpenText(path))
            {
                ParseMetadata(file);
                ParseData(file);
            }
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
            Data = new double[NumberOfColumns][];

            for (var row = 0; row < NumberOfRows; row++)
            {
                var line = file.ReadLine()
                    .Trim()
                    .Split(' ')
                    .Select(double.Parse).ToArray();

                for (var column = 0; column < NumberOfColumns; column++)
                {
                    if (Data[column] == null)
                    {
                        Data[column] = new double[NumberOfRows];
                    }

                    Data[column][NumberOfRows - 1 - row] = line[column];
                }
            }
        }

        public double GetValue(Coordinate coord)
        {
            if (coord.X < 0 || coord.X >= NumberOfColumns || coord.Y < 0 || coord.Y >= NumberOfRows)
            {
                return NoDataValue;
            }

            return Data[coord.X][coord.Y];
        }

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

        private Coordinate GetFieldFromGps(Gps gps)
        {
            var x = (int) ((gps.Longitude - XLowerLeftCorner) / CellSize);
            var y = (int) ((gps.Latitude - YLowerLeftCorner) / CellSize);

            return new Coordinate(x, y);
        }

        public string GetMetadata()
        {
            return "ncols " + NumberOfColumns +
                   "\nnrows " + NumberOfRows +
                   "\nxllcorner " + XLowerLeftCorner +
                   "\nyllcorner " + YLowerLeftCorner +
                   "\ncellsize " + CellSize +
                   "\nnodata_value " + NoDataValue;
        }

        public string GetData()
        {
            var str = "";
            for (var row = 0; row < NumberOfRows; row++)
            {
                for (var column = 0; column < NumberOfColumns; column++)
                {
                    str += Data[column][row] + " ";
                }
                str = str.Trim();

                if (row < NumberOfRows - 1)
                {
                    str += "\n";
                }
            }

            return str;
        }

        public override string ToString()
        {
            return GetMetadata() + "\n" + GetData();
        }
    }
}