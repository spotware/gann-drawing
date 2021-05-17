using cAlgo.API;
using System.Linq;

namespace cAlgo.Helpers
{
    public static class ChartPatternsVisibilityExtensions
    {
        public static void ChangePatternsVisibility(this Chart chart, bool isHidden)
        {
            var chartObjects = chart.Objects.ToArray();

            foreach (var chartObject in chartObjects)
            {
                if (!chartObject.IsPattern()) continue;

                chartObject.IsHidden = isHidden;
            }
        }
    }
}