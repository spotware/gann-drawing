using cAlgo.API;
using System;

namespace cAlgo.Helpers
{
    public static class ChartRectangleExtensions
    {
        public static double GetPriceDelta(this ChartRectangle rectangle)
        {
            return Math.Abs(rectangle.Y2 - rectangle.Y1);
        }

        public static double GetTopPrice(this ChartRectangle rectangle)
        {
            return rectangle.Y2 > rectangle.Y1 ? rectangle.Y2 : rectangle.Y1;
        }

        public static double GetBottomPrice(this ChartRectangle rectangle)
        {
            return rectangle.Y2 > rectangle.Y1 ? rectangle.Y1 : rectangle.Y2;
        }

        public static DateTime GetStartTime(this ChartRectangle rectangle)
        {
            return rectangle.Time1 > rectangle.Time2 ? rectangle.Time2 : rectangle.Time1;
        }

        public static DateTime GetEndTime(this ChartRectangle rectangle)
        {
            return rectangle.Time1 > rectangle.Time2 ? rectangle.Time1 : rectangle.Time2;
        }

        public static double GetStartBarIndex(this ChartRectangle rectangle, Bars bars)
        {
            return bars.GetBarIndex(rectangle.GetStartTime());
        }

        public static double GetEndBarIndex(this ChartRectangle rectangle, Bars bars)
        {
            return bars.GetBarIndex(rectangle.GetEndTime());
        }

        public static double GetBarsNumber(this ChartRectangle rectangle, Bars bars)
        {
            var startX = rectangle.GetStartTime();
            var endX = rectangle.GetEndTime();

            var startBarIndex = bars.GetBarIndex(startX);
            var endBarIndex = bars.GetBarIndex(endX);

            return Math.Round(endBarIndex - startBarIndex, 2);
        }

        public static TimeSpan GetTimeDelta(this ChartRectangle rectangle)
        {
            return rectangle.GetEndTime() - rectangle.GetStartTime();
        }

        public static double GetPriceToBarsRatio(this ChartRectangle rectangle, Bars bars)
        {
            var verticalDelta = rectangle.GetPriceDelta();

            var barsNumnber = rectangle.GetBarsNumber(bars);

            return Math.Round(verticalDelta / barsNumnber, 10);
        }
    }
}