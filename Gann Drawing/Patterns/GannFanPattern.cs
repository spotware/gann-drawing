using cAlgo.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace cAlgo.Patterns
{
    public class GannFanPattern : FanPatternBase
    {
        public GannFanPattern(PatternConfig config, SideFanSettings[] sideFanSettings, FanSettings mainFanSettings) : base("Gann Fan", config, sideFanSettings, mainFanSettings)
        {
        }

        protected override void DrawLabels()
        {
            if (MainFanLine == null || SideFanLines.Count < 8) return;

            DrawLabels(MainFanLine, SideFanLines, Id);
        }

        private void DrawLabels(ChartTrendLine mainFan, Dictionary<double, ChartTrendLine> sideFans, long id)
        {
            DrawLabelText("1/1", mainFan.Time2, mainFan.Y2, id, fontSize: 10, objectNameKey: "1x1");

            foreach (var fanSettings in SideFanSettings)
            {
                ChartTrendLine fanLine;

                if (!sideFans.TryGetValue(fanSettings.Percent, out fanLine)) continue;

                DrawLabelText(fanSettings.Name.Replace('x', '/'), fanLine.Time2, fanLine.Y2, id, fontSize: 10, objectNameKey: fanSettings.Name);
            }
        }

        protected override void UpdateLabels(long id, ChartObject chartObject, ChartText[] labels, ChartObject[] patternObjects)
        {
            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            if (trendLines == null) return;

            var mainFan = trendLines.FirstOrDefault(iLine => iLine.Name.IndexOf("MainFan", StringComparison.OrdinalIgnoreCase) > -1);

            if (mainFan == null) return;

            var sideFans = trendLines.Where(iLine => iLine.Name.IndexOf("SideFan", StringComparison.OrdinalIgnoreCase) > -1).ToDictionary(iLine => double.Parse(iLine.Name.Split('_').Last(), CultureInfo.InvariantCulture));

            if (labels.Length == 0)
            {
                DrawLabels(mainFan, sideFans, id);

                return;
            }

            foreach (var label in labels)
            {
                var labelFanName = label.Name.Split('_').Last();

                ChartTrendLine line;

                if (labelFanName.Equals("1x1", StringComparison.OrdinalIgnoreCase))
                {
                    line = mainFan;
                }
                else
                {
                    var fanSettings = SideFanSettings.FirstOrDefault(iFanSettings => iFanSettings.Name.Equals(labelFanName, StringComparison.OrdinalIgnoreCase));

                    if (fanSettings == null || !sideFans.TryGetValue(fanSettings.Percent, out line)) continue;
                }

                label.Time = line.Time2;
                label.Y = line.Y2;
            }
        }
    }
}