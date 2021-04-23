using cAlgo.API;
using System;
using System.Linq;

namespace cAlgo.Helpers
{
    public static class DataSeriesAverageDistanceExtension
    {
        public static double GetAverageDistance(this DataSeries dataSeries, int period)
        {
            return dataSeries.Skip(dataSeries.Count - period).Zip(dataSeries.Skip(dataSeries.Count - (period - 1)), (firstClose, secondClose) => Math.Abs(firstClose - secondClose)).Average();
        }
    }
}