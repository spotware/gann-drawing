using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.Helpers
{
    public static class SymbolTradingSessionExtensions
    {
        public static DayOfWeek[] GetSymbolTradingDays(this Symbol symbol)
        {
            var result = new List<DayOfWeek>();

            foreach (var session in symbol.MarketHours.Sessions)
            {
                result.Add(session.StartDay);
                result.Add(session.EndDay);
            }

            return result.Distinct().ToArray();
        }

        public static bool IsInTradingTime(this Symbol symbol, DateTime time)
        {
            return symbol.MarketHours.Sessions.Any(iSession => (time.DayOfWeek == iSession.StartDay && time.TimeOfDay >= iSession.StartTime) || (time.DayOfWeek == iSession.EndDay && time.TimeOfDay <= iSession.EndTime));
        }

        public static TimeSpan GetOutsideTradingTimeAmount(this Symbol symbol, DateTime startTime, DateTime endTime, TimeSpan interval)
        {
            var result = TimeSpan.FromMinutes(0);

            for (var currentTime = endTime; currentTime > startTime; currentTime = currentTime.Add(-interval))
            {
                if (symbol.IsInTradingTime(currentTime)) continue;

                result = result.Add(interval);
            }

            return result;
        }
    }
}