using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.Patterns
{
    public class GannSquarePattern : PatternBase
    {
        private ChartRectangle _rectangle;

        private ChartTrendLine[] _horizontalTrendLines;
        private ChartTrendLine[] _verticalTrendLines;

        private readonly Dictionary<string, ChartTrendLine> _fans = new Dictionary<string, ChartTrendLine>();
        private readonly GannSquareSettings _settings;

        public GannSquarePattern(PatternConfig config, GannSquareSettings settings) : base("Gann Square", config)
        {
            _settings = settings;
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.Rectangle) return;

            var rectangle = updatedChartObject as ChartRectangle;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>();

            var horizontalLines = trendLines.Where(iTrendLine => iTrendLine.Name.IndexOf("HorizontalLine", StringComparison.OrdinalIgnoreCase) > -1).ToArray();

            DrawOrUpdateHorizontalLines(rectangle, horizontalLines);

            var verticalLines = trendLines.Where(iTrendLine => iTrendLine.Name.IndexOf("VerticalLine", StringComparison.OrdinalIgnoreCase) > -1).ToArray();

            DrawOrUpdateVerticalLines(rectangle, verticalLines);

            UpdateFans(rectangle, trendLines.Where(iTrendLine => iTrendLine.Name.Split('_').Last().IndexOf("Fan", StringComparison.OrdinalIgnoreCase) > -1).ToArray());
        }

        private void UpdateFans(ChartRectangle rectangle, ChartTrendLine[] fans)
        {
            var startTime = rectangle.GetStartTime();
            var endTime = rectangle.GetEndTime();

            var topPrice = rectangle.GetTopPrice();
            var bottomPrice = rectangle.GetBottomPrice();

            var rectanglePriceDelta = rectangle.GetPriceDelta();
            var rectangleBarsNumber = rectangle.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var startBarIndex = rectangle.GetStartBarIndex(Chart.Bars, Chart.Symbol);

            foreach (var fan in fans)
            {
                var fanName = fan.Name.Split('.').Last();

                fan.Time1 = startTime;
                fan.Y1 = bottomPrice;

                switch (fanName)
                {
                    case "1x1":
                        fan.Time2 = endTime;
                        fan.Y2 = topPrice;
                        break;

                    case "1x2":
                    case "1x3":
                    case "1x4":
                    case "1x5":
                    case "1x8":
                        {
                            fan.Y2 = topPrice;

                            switch (fanName)
                            {
                                case "1x2":
                                    fan.Time2 = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 2), Chart.Symbol);
                                    break;

                                case "1x3":
                                    fan.Time2 = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 3), Chart.Symbol);
                                    break;

                                case "1x4":
                                    fan.Time2 = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 4), Chart.Symbol);
                                    break;

                                case "1x5":
                                    fan.Time2 = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 5), Chart.Symbol);
                                    break;

                                case "1x8":
                                    fan.Time2 = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 8), Chart.Symbol);
                                    break;
                            }

                            break;
                        }
                    case "2x1":
                    case "3x1":
                    case "4x1":
                    case "5x1":
                    case "8x1":
                        {
                            fan.Time2 = endTime;

                            switch (fanName)
                            {
                                case "2x1":
                                    fan.Y2 = bottomPrice + (rectanglePriceDelta / 2);
                                    break;

                                case "3x1":
                                    fan.Y2 = bottomPrice + (rectanglePriceDelta / 3);
                                    break;

                                case "4x1":
                                    fan.Y2 = bottomPrice + (rectanglePriceDelta / 4);
                                    break;

                                case "5x1":
                                    fan.Y2 = bottomPrice + (rectanglePriceDelta / 5);
                                    break;

                                case "8x1":
                                    fan.Y2 = bottomPrice + (rectanglePriceDelta / 8);
                                    break;
                            }

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException("level", "The fan name is outside valid range");
                }
            }
        }

        protected override void OnDrawingStopped()
        {
            _rectangle = null;
            _horizontalTrendLines = null;
            _verticalTrendLines = null;

            _fans.Clear();
        }

        protected override void OnMouseUp(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber == 2)
            {
                FinishDrawing();

                return;
            }

            if (_rectangle == null)
            {
                var name = GetObjectName("Rectangle");

                _rectangle = Chart.DrawRectangle(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _settings.RectangleColor, _settings.RectangleThickness, _settings.RectangleStyle);

                _rectangle.IsInteractive = true;
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber > 1 || _rectangle == null) return;

            _rectangle.Time2 = obj.TimeValue;
            _rectangle.Y2 = obj.YValue;

            _horizontalTrendLines = new ChartTrendLine[4];

            DrawOrUpdateHorizontalLines(_rectangle, _horizontalTrendLines);

            _verticalTrendLines = new ChartTrendLine[4];

            DrawOrUpdateVerticalLines(_rectangle, _verticalTrendLines);

            DrawFans(_rectangle);
        }

        private void DrawFans(ChartRectangle rectangle)
        {
            var startTime = rectangle.GetStartTime();
            var endTime = rectangle.GetEndTime();

            var topPrice = rectangle.GetTopPrice();
            var bottomPrice = rectangle.GetBottomPrice();

            var rectanglePriceDelta = rectangle.GetPriceDelta();
            var rectangleBarsNumber = rectangle.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var startBarIndex = rectangle.GetStartBarIndex(Chart.Bars, Chart.Symbol);

            var levels = new[] { 1, 2, 3, 4, 5, 8, -2, -3, -4, -5, -8 };

            foreach (var level in levels)
            {
                string name = null;
                DateTime secondTime = endTime;
                double secondPrice = topPrice;

                switch (level)
                {
                    case 1:
                        name = "1x1";
                        secondTime = endTime;
                        secondPrice = topPrice;
                        break;

                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 8:
                        {
                            name = string.Format("1x{0}", level);
                            secondPrice = topPrice;

                            switch (level)
                            {
                                case 2:
                                    secondTime = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 2), Chart.Symbol);
                                    break;

                                case 3:
                                    secondTime = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 3), Chart.Symbol);
                                    break;

                                case 4:
                                    secondTime = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 4), Chart.Symbol);
                                    break;

                                case 5:
                                    secondTime = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 5), Chart.Symbol);
                                    break;

                                case 8:
                                    secondTime = Chart.Bars.GetOpenTime(startBarIndex + (rectangleBarsNumber / 8), Chart.Symbol);
                                    break;
                            }

                            break;
                        }
                    case -2:
                    case -3:
                    case -4:
                    case -5:
                    case -8:
                        {
                            name = string.Format("{0}x1", Math.Abs(level));
                            secondTime = endTime;

                            switch (level)
                            {
                                case -2:
                                    secondPrice = bottomPrice + (rectanglePriceDelta / 2);
                                    break;

                                case -3:
                                    secondPrice = bottomPrice + (rectanglePriceDelta / 3);
                                    break;

                                case -4:
                                    secondPrice = bottomPrice + (rectanglePriceDelta / 4);
                                    break;

                                case -5:
                                    secondPrice = bottomPrice + (rectanglePriceDelta / 5);
                                    break;

                                case -8:
                                    secondPrice = bottomPrice + (rectanglePriceDelta / 8);
                                    break;
                            }

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException("level", "The fan level is outside valid range");
                }

                if (string.IsNullOrWhiteSpace(name)) continue;

                var objectName = GetObjectName(string.Format("Fan.{0}", name));

                var trendLine = Chart.DrawTrendLine(objectName, startTime, bottomPrice, secondTime, secondPrice, _settings.FansColor, _settings.FansThickness, _settings.FansStyle);

                trendLine.IsInteractive = true;
                trendLine.IsLocked = true;

                _fans[name] = trendLine;
            }
        }

        private void DrawOrUpdateHorizontalLines(ChartRectangle rectangle, ChartTrendLine[] horizontalLines)
        {
            var startTime = rectangle.GetStartTime();
            var endTime = rectangle.GetEndTime();

            var verticalDelta = rectangle.GetPriceDelta();

            var lineLevels = new double[]
            {
                verticalDelta * 0.2,
                verticalDelta * 0.4,
                verticalDelta * 0.6,
                verticalDelta * 0.8,
            };

            for (int i = 0; i < lineLevels.Length; i++)
            {
                var level = rectangle.Y2 > rectangle.Y1 ? rectangle.Y1 + lineLevels[i] : rectangle.Y1 - lineLevels[i];

                var horizontalLine = horizontalLines[i];

                if (horizontalLine == null)
                {
                    var objectName = GetObjectName(string.Format("HorizontalLine{0}", i + 1));

                    horizontalLines[i] = Chart.DrawTrendLine(objectName, startTime, level, endTime, level, _settings.PriceLevelsColor, _settings.PriceLevelsThickness, _settings.PriceLevelsStyle);

                    horizontalLines[i].IsInteractive = true;
                    horizontalLines[i].IsLocked = true;
                }
                else
                {
                    horizontalLine.Time1 = startTime;
                    horizontalLine.Time2 = endTime;

                    horizontalLine.Y1 = level;
                    horizontalLine.Y2 = level;
                }
            }
        }

        private void DrawOrUpdateVerticalLines(ChartRectangle rectangle, ChartTrendLine[] verticalLines)
        {
            var startBarIndex = rectangle.GetStartBarIndex(Chart.Bars, Chart.Symbol);

            var barsNumber = rectangle.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var lineLevels = new double[]
            {
                barsNumber * 0.2,
                barsNumber * 0.4,
                barsNumber * 0.6,
                barsNumber * 0.8,
            };

            var rectangleEndTime = rectangle.GetEndTime();

            for (int i = 0; i < lineLevels.Length; i++)
            {
                var barIndex = startBarIndex + lineLevels[i];

                var time = Chart.Bars.GetOpenTime(barIndex, Chart.Symbol);

                if (time > rectangleEndTime)
                {
                    time = rectangleEndTime;
                }

                var verticalLine = verticalLines[i];

                if (verticalLine == null)
                {
                    var objectName = GetObjectName(string.Format("VerticalLine{0}", i + 1));

                    verticalLines[i] = Chart.DrawTrendLine(objectName, time, rectangle.Y1, time, rectangle.Y2, _settings.TimeLevelsColor, _settings.TimeLevelsThickness, _settings.TimeLevelsStyle);

                    verticalLines[i].IsInteractive = true;
                    verticalLines[i].IsLocked = true;
                }
                else
                {
                    verticalLine.Time1 = time;
                    verticalLine.Time2 = time;

                    verticalLine.Y1 = rectangle.Y1;
                    verticalLine.Y2 = rectangle.Y2;
                }
            }
        }

        protected override void DrawLabels()
        {
            if (_rectangle == null) return;

            DrawLabels(_rectangle, Id);
        }

        private void DrawLabels(ChartRectangle rectangle, long id)
        {
            DrawLabelText(Math.Round(rectangle.GetPriceDelta(), Chart.Symbol.Digits).ToNonScientificString(), rectangle.GetStartTime(), rectangle.GetTopPrice(), id, objectNameKey: "Price", fontSize: 10);
            DrawLabelText(rectangle.GetBarsNumber(Chart.Bars, Chart.Symbol).ToString(), rectangle.GetEndTime(), rectangle.GetBottomPrice(), id, objectNameKey: "BarsNumber", fontSize: 10);
            DrawLabelText(rectangle.GetPriceToBarsRatio(Chart.Bars, Chart.Symbol).ToNonScientificString(), rectangle.GetEndTime(), rectangle.GetTopPrice(), id, objectNameKey: "PriceToBarsRatio", fontSize: 10);
        }

        protected override void UpdateLabels(long id, ChartObject chartObject, ChartText[] labels, ChartObject[] patternObjects)
        {
            var rectangle = patternObjects.FirstOrDefault(iObject => iObject is ChartRectangle) as ChartRectangle;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>();

            if (rectangle == null) return;

            if (labels.Length == 0)
            {
                DrawLabels(rectangle, id);

                return;
            }

            foreach (var label in labels)
            {
                var labelKey = label.Name.Split('_').Last();

                switch (labelKey)
                {
                    case "Price":
                        label.Text = Math.Round(rectangle.GetPriceDelta(), Chart.Symbol.Digits).ToNonScientificString();
                        label.Time = rectangle.GetStartTime();
                        label.Y = rectangle.GetTopPrice();
                        break;

                    case "BarsNumber":
                        label.Text = rectangle.GetBarsNumber(Chart.Bars, Chart.Symbol).ToString();
                        label.Time = rectangle.GetEndTime();
                        label.Y = rectangle.GetBottomPrice();
                        break;

                    case "PriceToBarsRatio":
                        label.Text = rectangle.GetPriceToBarsRatio(Chart.Bars, Chart.Symbol).ToNonScientificString();
                        label.Time = rectangle.GetEndTime();
                        label.Y = rectangle.GetTopPrice();
                        break;
                }
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _rectangle };
        }
    }
}