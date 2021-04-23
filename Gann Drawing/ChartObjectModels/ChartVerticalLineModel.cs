using cAlgo.API;
using System;

namespace cAlgo.ChartObjectModels
{
    public class ChartVerticalLineModel : ChartObjectBaseModel
    {
        public DateTime Time { get; set; }

        public string ColorHex { get; set; }

        public int Thickness { get; set; }

        public LineStyle LineStyle { get; set; }
    }
}