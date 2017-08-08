using System;
using System.IO;
using System.Linq;

namespace GisPerformanceDotnetCore.tools
{
    public class EsriAsciiParser
    {
        public int NumberOfColumns { get; }

        public int NumberOfRows { get; }

        public double XLowerLeftCorner { get; }

        public double YLowerLeftCorner { get; }

        public double CellSize { get; }

        public int NoDataValue { get; }

        public int[][] Data { get; }


        public EsriAsciiParser(string path)
        {
            using (var file = File.OpenText(path))
            {
                try
                {
                    NumberOfColumns = int.Parse(file.ReadLine().Substring("ncols".Length + 1));
                    NumberOfRows = int.Parse(file.ReadLine().Substring("nrows".Length + 1));
                    XLowerLeftCorner = double.Parse(file.ReadLine().Substring("xllcorner".Length + 1));
                    YLowerLeftCorner = double.Parse(file.ReadLine().Substring("yllcorner".Length + 1));
                    CellSize = double.Parse(file.ReadLine().Substring("cellsize".Length + 1));
                    NoDataValue = int.Parse(file.ReadLine().Substring("nodata_value".Length + 1));
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.StackTrace);
                    throw new FormatException("Esri ascii file not valid!");
                }

                Data = new int[NumberOfColumns][];

                // Fill data
                for (var row = 0; row < NumberOfRows; row++)
                {
                    var line = file.ReadLine().Trim()
                        .Split(' ')
                        .Select(int.Parse).ToArray();

                    for (var column = 0; column < NumberOfColumns; column++)
                    {
                        if (Data[column] == null)
                        {
                            Data[column] = new int[NumberOfRows];
                        }

                        Data[column][NumberOfRows - 1 - row] = line[column];
                    }
                }
            }
        }

        public int GetValueFromGps(double lng, double lat)
        {
            if (lng < XLowerLeftCorner || lng > XLowerLeftCorner + NumberOfColumns * CellSize ||
                lat < YLowerLeftCorner || lat > YLowerLeftCorner + NumberOfRows * CellSize)
            {
                return -9999;
            }

            var xIndex = (int) ((lng - XLowerLeftCorner) / CellSize);
            var yIndex = (int) ((lat - YLowerLeftCorner) / CellSize);

            return GetValue(xIndex, yIndex);
        }

        public int GetValue(int x, int y)
        {
            if (x < 0 || x >= NumberOfColumns || y < 0 || y >= NumberOfRows)
            {
                return -9999;
            }

            return Data[x][y];
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