using cAlgo.API;
using cAlgo.API.Internals;
using System;
using System.Linq;

namespace cAlgo.Helpers
{
    public static class BarsOpenTimeExtensions
    {
        public static DateTime GetOpenTime(this Bars bars, double barIndex, Symbol symbol)
        {
            var currentIndex = bars.Count - 1;

            var timeDiff = bars.GetTimeDiff();

            var indexDiff = barIndex - currentIndex;

            var result = indexDiff <= 0 ? bars.OpenTimes[(int)barIndex] : bars.OpenTimes[currentIndex];

            if (indexDiff > 0)
            {
                for (var i = 1; i <= indexDiff; i++)
                {
                    do
                    {
                        result = result.Add(timeDiff);
                    }
                    while (!symbol.IsInTradingTime(result));
                }
            }

            return result;
        }

        public static double GetBarIndex(this Bars bars, DateTime time, Symbol symbol)
        {
            if (time <= bars.OpenTimes.LastValue) return bars.OpenTimes.GetIndexByTime(time);

            var timeDiff = bars.GetTimeDiff();

            var outsideTradingTime = symbol.GetOutsideTradingTimeAmount(bars.OpenTimes.LastValue, time, timeDiff);

            var timeDiffWithLastTime = (time - bars.OpenTimes.LastValue).Add(-outsideTradingTime);

            var futureIndex = (bars.Count - 1) + timeDiffWithLastTime.TotalHours / timeDiff.TotalHours;

            return futureIndex;
        }

        public static TimeSpan GetTimeDiff(this Bars bars)
        {
            var index = bars.Count - 1;

            if (index < 4)
            {
                throw new InvalidOperationException("Not enough data in market series to calculate the time difference");
            }

            var timeDiffs = new TimeSpan[4];

            for (var i = 0; i < 4; i++)
            {
                timeDiffs[i] = bars.OpenTimes[index - i] - bars.OpenTimes[index - i - 1];
            }

            return timeDiffs.GroupBy(diff => diff).OrderBy(diffGroup => diffGroup.Count()).Last().First();
        }
    }
}