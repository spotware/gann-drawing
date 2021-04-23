using cAlgo.API;
using System;

namespace cAlgo.ChartObjectModels
{
    public class ChartTextModel : ChartObjectBaseModel
    {
        public DateTime Time { get; set; }

        public double Y { get; set; }

        public string ColorHex { get; set; }

        public string Text { get; set; }

        public double FontSize { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderlined { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }
    }
}