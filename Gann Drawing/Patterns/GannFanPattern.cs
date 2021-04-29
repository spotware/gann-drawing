using cAlgo.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.Patterns
{
    public class GannFanPattern : PatternBase
    {
        private ChartTrendLine _mainFan;

        private readonly Dictionary<string, ChartTrendLine> _sideFans = new Dictionary<string, ChartTrendLine>();

        private readonly double[] _percents = new[] { 0.416, 0.583, 0.666, 0.833 };

        private readonly int[] _fanLevels = new[] { 2, 3, 4, 8, -2, -3, -4, -8 };
        private readonly GannFanSettings _settings;

        public GannFanPattern(PatternConfig config, GannFanSettings settings) : base("Gann Fan", config)
        {
            _settings = settings;
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.TrendLine) return;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            var mainFan = trendLines.FirstOrDefault(iLine => iLine.Name.IndexOf("1x1", StringComparison.OrdinalIgnoreCase) > -1);

            var sideFans = trendLines.Where(iLine => iLine != mainFan).ToDictionary(iLine => iLine.Name.Split('_').Last());

            UpdateFans(mainFan, sideFans);
        }

        private void UpdateFans(ChartTrendLine mainFan, Dictionary<string, ChartTrendLine> sideFans)
        {
            var mainFanPriceDelta = Math.Abs(mainFan.Y2 - mainFan.Y1);

            for (var iLevel = 0; iLevel < _fanLevels.Length; iLevel++)
            {
                var level = _fanLevels[iLevel];

                string name;
                double y2;

                if (level > 0)
                {
                    name = string.Format("1x{0}", level);

                    var yAmount = mainFanPriceDelta * _percents[iLevel];

                    y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 + yAmount : mainFan.Y2 - yAmount;
                }
                else
                {
                    name = string.Format("{0}x1", Math.Abs(level));

                    var yAmount = mainFanPriceDelta * _percents[iLevel - 4];

                    y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 - yAmount : mainFan.Y2 + yAmount;
                }

                ChartTrendLine fanLine;

                if (!sideFans.TryGetValue(name, out fanLine)) continue;

                fanLine.Time1 = mainFan.Time1;
                fanLine.Time2 = mainFan.Time2;

                fanLine.Y1 = mainFan.Y1;
                fanLine.Y2 = y2;
            }
        }

        protected override void OnDrawingStopped()
        {
            _mainFan = null;

            _sideFans.Clear();
        }

        protected override void OnMouseUp(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber == 2)
            {
                FinishDrawing();

                return;
            }

            if (_mainFan == null)
            {
                var name = GetObjectName("1x1");

                _mainFan = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _settings.OneColor, _settings.OneThickness, _settings.OneStyle);

                _mainFan.IsInteractive = true;
                _mainFan.ExtendToInfinity = true;
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber > 1 || _mainFan == null) return;

            _mainFan.Time2 = obj.TimeValue;
            _mainFan.Y2 = obj.YValue;

            DrawFans(_mainFan);
        }

        private void DrawFans(ChartTrendLine mainFan)
        {
            var mainFanPriceDelta = Math.Abs(mainFan.Y2 - mainFan.Y1);

            for (var iLevel = 0; iLevel < _fanLevels.Length; iLevel++)
            {
                var level = _fanLevels[iLevel];

                string name;
                double y2;

                if (level > 0)
                {
                    name = string.Format("1x{0}", level);

                    var yAmount = mainFanPriceDelta * _percents[iLevel];

                    y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 + yAmount : mainFan.Y2 - yAmount;
                }
                else
                {
                    name = string.Format("{0}x1", Math.Abs(level));

                    var yAmount = mainFanPriceDelta * _percents[iLevel - 4];

                    y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 - yAmount : mainFan.Y2 + yAmount;
                }

                var objectName = GetObjectName(name);

                Color color;
                int thickness;
                LineStyle lineStyle;

                switch (name)
                {
                    case "1x2":
                    case "2x1":
                        color = _settings.TwoColor;
                        thickness = _settings.TwoThickness;
                        lineStyle = _settings.TwoStyle;
                        break;

                    case "1x3":
                    case "3x1":
                        color = _settings.ThreeColor;
                        thickness = _settings.ThreeThickness;
                        lineStyle = _settings.ThreeStyle;
                        break;

                    case "1x4":
                    case "4x1":
                        color = _settings.FourColor;
                        thickness = _settings.FourThickness;
                        lineStyle = _settings.FourStyle;
                        break;

                    case "1x8":
                    case "8x1":
                        color = _settings.EightColor;
                        thickness = _settings.EightThickness;
                        lineStyle = _settings.EightStyle;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(name, "Invalid fan name");
                }

                var trendLine = Chart.DrawTrendLine(objectName, mainFan.Time1, mainFan.Y1, mainFan.Time2, y2, color, thickness, lineStyle);

                trendLine.IsInteractive = true;
                trendLine.IsLocked = true;
                trendLine.ExtendToInfinity = true;

                _sideFans[name] = trendLine;
            }
        }

        protected override void DrawLabels()
        {
            if (_mainFan == null || _sideFans.Count < 8) return;

            DrawLabels(_mainFan, _sideFans, Id);
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

            var sideFans = trendLines.Where(iLine => iLine != mainFan).ToDictionary(iLine => iLine.Name.Split('_').Last());

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

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _mainFan };
        }
    }
}