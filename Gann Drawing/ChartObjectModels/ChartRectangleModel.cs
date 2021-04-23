using System;

namespace cAlgo.ChartObjectModels
{
    public class ChartRectangleModel : ChartShapeModel
    {
        public DateTime Time1 { get; set; }

        public DateTime Time2 { get; set; }

        public double Y1 { get; set; }

        public double Y2 { get; set; }
    }
}