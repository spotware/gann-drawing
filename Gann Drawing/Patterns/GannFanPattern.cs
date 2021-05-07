using cAlgo.API;
using System;
using System.Collections.Generic;
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

        private void DrawLabels(ChartTrendLine mainFan, Dictionary<string, ChartTrendLine> sideFans, long id)
        {
            DrawLabelText("1/1", mainFan.Time2, mainFan.Y2, id, fontSize: 10);

            DrawLabelText("1/2", sideFans["1x2"].Time2, sideFans["1x2"].Y2, id, fontSize: 10);
            DrawLabelText("1/3", sideFans["1x3"].Time2, sideFans["1x3"].Y2, id, fontSize: 10);
            DrawLabelText("1/4", sideFans["1x4"].Time2, sideFans["1x4"].Y2, id, fontSize: 10);
            DrawLabelText("1/8", sideFans["1x8"].Time2, sideFans["1x8"].Y2, id, fontSize: 10);

            DrawLabelText("2/1", sideFans["2x1"].Time2, sideFans["2x1"].Y2, id, fontSize: 10);
            DrawLabelText("3/1", sideFans["3x1"].Time2, sideFans["3x1"].Y2, id, fontSize: 10);
            DrawLabelText("4/1", sideFans["4x1"].Time2, sideFans["4x1"].Y2, id, fontSize: 10);
            DrawLabelText("8/1", sideFans["8x1"].Time2, sideFans["8x1"].Y2, id, fontSize: 10);
        }

        protected override void UpdateLabels(long id, ChartObject chartObject, ChartText[] labels, ChartObject[] patternObjects)
        {
            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            if (trendLines == null) return;

            var mainFan = trendLines.FirstOrDefault(iLine => iLine.Name.IndexOf("1x1", StringComparison.OrdinalIgnoreCase) > -1);

            if (mainFan == null) return;

            var sideFans = trendLines.Where(iLine => iLine.Name.IndexOf("SideFan", StringComparison.OrdinalIgnoreCase) > -1).ToDictionary(iLine => iLine.Name.Split('_').Last());

            if (labels.Length == 0)
            {
                DrawLabels(mainFan, sideFans, id);

                return;
            }

            foreach (var label in labels)
            {
                ChartTrendLine line;

                if (label.Text.Equals("1/1", StringComparison.OrdinalIgnoreCase))
                {
                    line = mainFan;
                }
                else
                {
                    var keyName = label.Text[0] == '1' ? string.Format("1x{0}", label.Text[2]) : string.Format("{0}x1", label.Text[0]);

                    if (!sideFans.TryGetValue(keyName, out line)) continue;
                }

                label.Time = line.Time2;
                label.Y = line.Y2;
            }
        }
    }
}