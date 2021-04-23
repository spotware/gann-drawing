using cAlgo.API;
using System;

namespace cAlgo.Helpers
{
    public static class ChartObjectsIsPatternExtension
    {
        public static bool IsPattern(this ChartObject chartObject)
        {
            return chartObject.Name.StartsWith("Pattern_", StringComparison.OrdinalIgnoreCase);
        }
    }
}