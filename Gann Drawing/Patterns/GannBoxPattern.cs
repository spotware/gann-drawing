using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Linq;

namespace cAlgo.Patterns
{
    public class GannBoxPattern : PatternBase
    {
        private readonly GannBoxSettings _settings;

        private ChartRectangle _rectangle;

        private ChartTrendLine[] _horizontalTrendLines;
        private ChartTrendLine[] _verticalTrendLines;

        public GannBoxPattern(PatternConfig config, GannBoxSettings settings) : base("Gann Box", config)
        {
            _settings = settings;
        }

        protected override ChartObject MainObject { get { return _rectangle; } }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.Rectangle) return;

            var rectangle = updatedChartObject as ChartRectangle;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>();

            var horizontalLines = trendLines.Where(iTrendLine => iTrendLine.Name.IndexOf("HorizontalLine", StringComparison.OrdinalIgnoreCase) > -1).ToArray();

            DrawOrUpdateHorizontalLines(rectangle, horizontalLines);

            var verticalLines = trendLines.Where(iTrendLine => iTrendLine.Name.IndexOf("VerticalLine", StringComparison.OrdinalIgnoreCase) > -1).ToArray();

            DrawOrUpdateVerticalLines(rectangle, verticalLines);
        }

        protected override void OnDrawingStopped()
        {
            _rectangle = null;
            _horizontalTrendLines = null;
            _verticalTrendLines = null;
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

            _horizontalTrendLines = new ChartTrendLine[5];

            DrawOrUpdateHorizontalLines(_rectangle, _horizontalTrendLines);

            _verticalTrendLines = new ChartTrendLine[5];

            DrawOrUpdateVerticalLines(_rectangle, _verticalTrendLines);
        }

        private void DrawOrUpdateHorizontalLines(ChartRectangle rectangle, ChartTrendLine[] horizontalLines)
        {
            DateTime startTime, endTime;

            if (rectangle.Time1 < rectangle.Time2)
            {
                startTime = rectangle.Time1;
                endTime = rectangle.Time2;
            }
            else
            {
                startTime = rectangle.Time2;
                endTime = rectangle.Time1;
            }

            var diff = Math.Abs(rectangle.Y2 - rectangle.Y1);

            var lineLevels = new double[]
            {
                diff * 0.25,
                diff * 0.382,
                diff * 0.5,
                diff * 0.618,
                diff * 0.75
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
            var rectangleFirstBarIndex = Chart.Bars.GetBarIndex(rectangle.Time1, Chart.Symbol);
            var rectangleSecondBarIndex = Chart.Bars.GetBarIndex(rectangle.Time2, Chart.Symbol);

            double startBarIndex, endBarIndex;

            if (rectangleFirstBarIndex < rectangleSecondBarIndex)
            {
                startBarIndex = rectangleFirstBarIndex;
                endBarIndex = rectangleSecondBarIndex;
            }
            else
            {
                startBarIndex = rectangleSecondBarIndex;
                endBarIndex = rectangleFirstBarIndex;
            }

            var diff = endBarIndex - startBarIndex;

            var lineLevels = new double[]
            {
                diff * 0.25,
                diff * 0.382,
                diff * 0.5,
                diff * 0.618,
                diff * 0.75
            };

            for (int i = 0; i < lineLevels.Length; i++)
            {
                var barIndex = startBarIndex + lineLevels[i];

                var time = Chart.Bars.GetOpenTime(barIndex, Chart.Symbol);

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
            if (_rectangle == null || _horizontalTrendLines == null || _verticalTrendLines == null) return;

            DrawLabels(_rectangle, _horizontalTrendLines, _verticalTrendLines, Id);
        }

        private void DrawLabels(ChartRectangle rectangle, ChartTrendLine[] horizontalLines, ChartTrendLine[] verticalLines, long id)
        {
            var timeDistance = -TimeSpan.FromHours(Chart.Bars.GetTimeDiff().TotalHours * 2);

            DrawLabelText("0", rectangle.Time1, rectangle.Y1, id, objectNameKey: "0.0", fontSize: 10);
            DrawLabelText("1", rectangle.Time1, rectangle.Y2, id, objectNameKey: "1.1", fontSize: 10);

            DrawLabelText("0", rectangle.Time1.Add(timeDistance), rectangle.Y1, id, objectNameKey: "0.2", fontSize: 10);
            DrawLabelText("1", rectangle.Time2.Add(timeDistance), rectangle.Y1, id, objectNameKey: "1.3", fontSize: 10);

            DrawLabelText("0", rectangle.Time2, rectangle.Y1, id, objectNameKey: "0.4", fontSize: 10);
            DrawLabelText("1", rectangle.Time2, rectangle.Y2, id, objectNameKey: "1.5", fontSize: 10);

            DrawLabelText("0", rectangle.Time1.Add(timeDistance), rectangle.Y2, id, objectNameKey: "0.6", fontSize: 10);
            DrawLabelText("1", rectangle.Time2.Add(timeDistance), rectangle.Y2, id, objectNameKey: "1.7", fontSize: 10);

            var minLength = Math.Min(horizontalLines.Length, verticalLines.Length);

            for (var i = 0; i < minLength; i++)
            {
                var horizontalLine = horizontalLines[i];
                var verticalLine = verticalLines[i];

                switch (i)
                {
                    case 0:
                        DrawLabelText("0.25", horizontalLine.Time1, horizontalLine.Y1, id, objectNameKey: "Horizontal1.0", fontSize: 10);
                        DrawLabelText("0.25", horizontalLine.Time2, horizontalLine.Y2, id, objectNameKey: "Horizontal1.1", fontSize: 10);

                        DrawLabelText("0.25", verticalLine.Time1, verticalLine.Y1, id, objectNameKey: "Vertical1.0", fontSize: 10);
                        DrawLabelText("0.25", verticalLine.Time2, verticalLine.Y2, id, objectNameKey: "Vertical1.1", fontSize: 10);
                        break;

                    case 1:
                        DrawLabelText("0.382", horizontalLine.Time1, horizontalLine.Y1, id, objectNameKey: "Horizontal2.0", fontSize: 10);
                        DrawLabelText("0.382", horizontalLine.Time2, horizontalLine.Y2, id, objectNameKey: "Horizontal2.1", fontSize: 10);

                        DrawLabelText("0.382", verticalLine.Time1, verticalLine.Y1, id, objectNameKey: "Vertical2.0", fontSize: 10);
                        DrawLabelText("0.382", verticalLine.Time2, verticalLine.Y2, id, objectNameKey: "Vertical2.1", fontSize: 10);
                        break;

                    case 2:
                        DrawLabelText("0.5", horizontalLine.Time1, horizontalLine.Y1, id, objectNameKey: "Horizontal3.0", fontSize: 10);
                        DrawLabelText("0.5", horizontalLine.Time2, horizontalLine.Y2, id, objectNameKey: "Horizontal3.1", fontSize: 10);

                        DrawLabelText("0.5", verticalLine.Time1, verticalLine.Y1, id, objectNameKey: "Vertical3.0", fontSize: 10);
                        DrawLabelText("0.5", verticalLine.Time2, verticalLine.Y2, id, objectNameKey: "Vertical3.1", fontSize: 10);
                        break;

                    case 3:
                        DrawLabelText("0.618", horizontalLine.Time1, horizontalLine.Y1, id, objectNameKey: "Horizontal4.0", fontSize: 10);
                        DrawLabelText("0.618", horizontalLine.Time2, horizontalLine.Y2, id, objectNameKey: "Horizontal4.1", fontSize: 10);

                        DrawLabelText("0.618", verticalLine.Time1, verticalLine.Y1, id, objectNameKey: "Vertical4.0", fontSize: 10);
                        DrawLabelText("0.618", verticalLine.Time2, verticalLine.Y2, id, objectNameKey: "Vertical4.1", fontSize: 10);
                        break;

                    case 4:
                        DrawLabelText("0.75", horizontalLine.Time1, horizontalLine.Y1, id, objectNameKey: "Horizontal5.0", fontSize: 10);
                        DrawLabelText("0.75", horizontalLine.Time2, horizontalLine.Y2, id, objectNameKey: "Horizontal5.1", fontSize: 10);

                        DrawLabelText("0.75", verticalLine.Time1, verticalLine.Y1, id, objectNameKey: "Vertical5.0", fontSize: 10);
                        DrawLabelText("0.75", verticalLine.Time2, verticalLine.Y2, id, objectNameKey: "Vertical5.1", fontSize: 10);
                        break;
                }
            }
        }

        protected override void UpdateLabels(long id, ChartObject chartObject, ChartText[] labels, ChartObject[] patternObjects)
        {
            var rectangle = patternObjects.FirstOrDefault(iObject => iObject is ChartRectangle) as ChartRectangle;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>();

            var horizontalLines = trendLines.Where(iTrendLine => iTrendLine.Name.IndexOf("HorizontalLine", StringComparison.OrdinalIgnoreCase) > -1).ToArray();

            var verticalLines = trendLines.Where(iTrendLine => iTrendLine.Name.IndexOf("VerticalLine", StringComparison.OrdinalIgnoreCase) > -1).ToArray();

            if (rectangle == null || horizontalLines == null || verticalLines == null) return;

            if (labels.Length == 0)
            {
                DrawLabels(rectangle, horizontalLines, verticalLines, id);

                return;
            }

            ChartTrendLine chartTrendLine;

            foreach (var label in labels)
            {
                var labelKey = label.Name.Split('_').Last();

                switch (labelKey)
                {
                    case "Horizontal1.0":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine1", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Horizontal1.1":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine1", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Horizontal2.0":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine2", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Horizontal2.1":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine2", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Horizontal3.0":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine3", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Horizontal3.1":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine3", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Horizontal4.0":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine4", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Horizontal4.1":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine4", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Horizontal5.0":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine5", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Horizontal5.1":
                        chartTrendLine = horizontalLines.First(iLine => iLine.Name.EndsWith("HorizontalLine5", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Vertical1.0":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine1", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Vertical1.1":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine1", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Vertical2.0":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine2", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Vertical2.1":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine2", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Vertical3.0":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine3", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Vertical3.1":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine3", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Vertical4.0":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine4", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Vertical4.1":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine4", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    case "Vertical5.0":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine5", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time1;
                        label.Y = chartTrendLine.Y1;
                        break;

                    case "Vertical5.1":
                        chartTrendLine = verticalLines.First(iLine => iLine.Name.EndsWith("VerticalLine5", StringComparison.OrdinalIgnoreCase));

                        label.Time = chartTrendLine.Time2;
                        label.Y = chartTrendLine.Y2;
                        break;

                    default:
                        {
                            var timeDistance = -TimeSpan.FromHours(Chart.Bars.GetTimeDiff().TotalHours * 2);

                            switch (labelKey)
                            {
                                case "0.0":
                                    label.Time = rectangle.Time1;
                                    label.Y = rectangle.Y1;
                                    break;

                                case "1.1":
                                    label.Time = rectangle.Time1;
                                    label.Y = rectangle.Y2;
                                    break;

                                case "0.2":
                                    label.Time = rectangle.Time1.Add(timeDistance);
                                    label.Y = rectangle.Y1;
                                    break;

                                case "1.3":
                                    label.Time = rectangle.Time2.Add(timeDistance);
                                    label.Y = rectangle.Y1;
                                    break;

                                case "0.4":
                                    label.Time = rectangle.Time2;
                                    label.Y = rectangle.Y1;
                                    break;

                                case "1.5":
                                    label.Time = rectangle.Time2;
                                    label.Y = rectangle.Y2;
                                    break;

                                case "0.6":
                                    label.Time = rectangle.Time1.Add(timeDistance);
                                    label.Y = rectangle.Y2;
                                    break;

                                case "1.7":
                                    label.Time = rectangle.Time2.Add(timeDistance);
                                    label.Y = rectangle.Y2;
                                    break;
                            }

                            break;
                        }
                }
            }
        }
    }
}