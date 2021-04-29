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
                var isDiffLessThanDay = timeDiff < TimeSpan.FromDays(1);

                for (var i = 1; i <= indexDiff; i++)
                {
                    result = result.Add(timeDiff);

                    if (isDiffLessThanDay)
                    {
                        while (!symbol.IsInTradingTime(result))
                        {
                            result = result.Add(timeDiff);
                        }
                    }
                }
            }

            return result;
        }

        public static double GetBarIndex(this Bars bars, DateTime time, Symbol symbol)
        {
            if (time <= bars.OpenTimes.LastValue) return bars.OpenTimes.GetIndexByTime(time);

            var timeDiff = bars.GetTimeDiff();

            var timeDiffWithLastTime = (time - bars.OpenTimes.LastValue);

            if (bars.TimeFrame < TimeFrame.Daily)
            {
                var outsideTradingTime = symbol.GetOutsideTradingTimeAmount(bars.OpenTimes.LastValue, time, timeDiff);

                timeDiffWithLastTime = timeDiffWithLastTime.Add(-outsideTradingTime);
            }

            var futureIndex = (bars.Count - 1) + timeDiffWithLastTime.TotalHours / timeDiff.TotalHours;

            return futureIndex;
        }

        public static TimeSpan GetTimeDiff(this Bars bars)
        {
            var index = bars.Count - 1;

            if (index < 10)
            {
                throw new InvalidOperationException("Not enough data in market series to calculate the time difference");
            }

            var timeDiffs = new TimeSpan[10];

            for (var i = 0; i < 10; i++)
            {
                timeDiffs[i] = bars.OpenTimes[index - i] - bars.OpenTimes[index - i - 1];
            }

            return timeDiffs.GroupBy(diff => diff).OrderBy(diffGroup => diffGroup.Count()).Last().First();
        }
    }
}