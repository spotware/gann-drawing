using cAlgo.API;
using System;
using System.Linq;

namespace cAlgo.Helpers
{
    public static class BarsOpenTimeExtensions
    {
        public static DateTime GetOpenTime(this Bars bars, double barIndex)
        {
            var currentIndex = bars.Count - 1;

            var timeDiff = bars.GetTimeDiff();

            var indexDiff = barIndex - currentIndex;

            var indexDiffAbs = Math.Abs(indexDiff);

            var result = indexDiff <= 0 ? bars.OpenTimes[(int)barIndex] : bars.OpenTimes[currentIndex];

            if (indexDiff > 0)
            {
                for (var i = 1; i <= indexDiffAbs; i++)
                {
                    do
                    {
                        result = result.Add(timeDiff);
                    }
                    while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday);
                }
            }

            var barIndexFraction = barIndex % 1;

            var barIndexFractionInMinutes = timeDiff.TotalMinutes * barIndexFraction;

            result = result.AddMinutes(barIndexFractionInMinutes);

            return result;
        }

        public static double GetBarIndex(this Bars bars, DateTime time)
        {
            if (time <= bars.OpenTimes.LastValue) return bars.OpenTimes.GetIndexByTime(time);

            var timeDiff = bars.GetTimeDiff();

            var weekendTime = GetWeekendTime(bars.OpenTimes.LastValue, time, timeDiff);

            var timeDiffWithLastTime = (time - bars.OpenTimes.LastValue).Add(-weekendTime);

            var futureIndex = (bars.Count - 1) + timeDiffWithLastTime.TotalHours / timeDiff.TotalHours;

            return futureIndex;
        }

        public static TimeSpan GetWeekendTime(DateTime startTime, DateTime endTime, TimeSpan interval)
        {
            var result = TimeSpan.FromMinutes(0);

            for (var currentTime = endTime; currentTime > startTime; currentTime = currentTime.Add(-interval))
            {
                if (currentTime.DayOfWeek != DayOfWeek.Saturday && currentTime.DayOfWeek != DayOfWeek.Sunday) continue;

                result = result.Add(interval);
            }

            return result;
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