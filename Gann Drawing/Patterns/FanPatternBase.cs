using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace cAlgo.Patterns
{
    public abstract class FanPatternBase : PatternBase
    {
        private ChartTrendLine _mainFanLine;

        private readonly Dictionary<double, ChartTrendLine> _sideFanLines = new Dictionary<double, ChartTrendLine>();

        private readonly SideFanSettings[] _sideFanSettings;

        private readonly FanSettings _mainFanSettings;

        private bool _callStopDrawing = true;

        public FanPatternBase(string name, PatternConfig config, SideFanSettings[] sideFanSettings, FanSettings mainFanSettings) : base(name, config)
        {
            _sideFanSettings = sideFanSettings;
            _mainFanSettings = mainFanSettings;
        }

        protected Dictionary<double, ChartTrendLine> SideFanLines
        {
            get
            {
                return _sideFanLines;
            }
        }

        protected ChartTrendLine MainFanLine
        {
            get
            {
                return _mainFanLine;
            }
        }

        protected SideFanSettings[] SideFanSettings
        {
            get
            {
                return _sideFanSettings;
            }
        }

        protected FanSettings MainFanSettings
        {
            get
            {
                return _mainFanSettings;
            }
        }

        protected bool CallStopDrawing
        {
            get
            {
                return _callStopDrawing;
            }
            set
            {
                _callStopDrawing = value;
            }
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.TrendLine) return;

            var trendLines = patternObjects.Where(iObject => iObject.ObjectType == ChartObjectType.TrendLine).Cast<ChartTrendLine>().ToArray();

            var mainFan = trendLines.First(iLine => iLine.Name.IndexOf("MainFan", StringComparison.OrdinalIgnoreCase) > -1);

            var sideFans = trendLines.Where(iLine => iLine.Name.IndexOf("SideFan", StringComparison.OrdinalIgnoreCase) > -1).ToDictionary(iLine => double.Parse(iLine.Name.Split('_').Last(), CultureInfo.InvariantCulture));

            UpdateSideFans(mainFan, sideFans);
        }

        protected virtual void UpdateSideFans(ChartTrendLine mainFan, Dictionary<double, ChartTrendLine> sideFans)
        {
            var startBarIndex = mainFan.GetStartBarIndex(Chart.Bars, Chart.Symbol);
            var endBarIndex = mainFan.GetEndBarIndex(Chart.Bars, Chart.Symbol);

            var barsNumber = mainFan.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var mainFanPriceDelta = mainFan.GetPriceDelta();

            for (var iFan = 0; iFan < SideFanSettings.Length; iFan++)
            {
                var fanSettings = SideFanSettings[iFan];

                double y2;
                DateTime time2;

                if (fanSettings.Percent < 0)
                {
                    var yAmount = mainFanPriceDelta * Math.Abs(fanSettings.Percent);

                    y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 - yAmount : mainFan.Y2 + yAmount;

                    time2 = mainFan.Time2;
                }
                else
                {
                    y2 = mainFan.Y2;

                    var barsPercent = barsNumber * fanSettings.Percent;

                    var barIndex = mainFan.Time2 > mainFan.Time1 ? endBarIndex - barsPercent : startBarIndex + barsPercent;

                    time2 = Chart.Bars.GetOpenTime(barIndex, Chart.Symbol);
                }

                ChartTrendLine fanLine;

                if (!sideFans.TryGetValue(fanSettings.Percent, out fanLine)) continue;

                fanLine.Time1 = mainFan.Time1;
                fanLine.Time2 = time2;

                fanLine.Y1 = mainFan.Y1;
                fanLine.Y2 = y2;
            }
        }

        protected override void OnDrawingStopped()
        {
            _mainFanLine = null;

            _sideFanLines.Clear();
        }

        protected override void OnMouseUp(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber == 2)
            {
                if (CallStopDrawing) FinishDrawing();

                return;
            }

            if (_mainFanLine == null)
            {
                var name = GetObjectName("MainFan");

                _mainFanLine = Chart.DrawTrendLine(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _mainFanSettings.Color, _mainFanSettings.Thickness, _mainFanSettings.Style);

                _mainFanLine.IsInteractive = true;
                _mainFanLine.ExtendToInfinity = true;
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (_mainFanLine == null) return;

            _mainFanLine.Time2 = obj.TimeValue;
            _mainFanLine.Y2 = obj.YValue;

            DrawSideFans(_mainFanLine);
        }

        protected virtual void DrawSideFans(ChartTrendLine mainFan)
        {
            var startBarIndex = mainFan.GetStartBarIndex(Chart.Bars, Chart.Symbol);
            var endBarIndex = mainFan.GetEndBarIndex(Chart.Bars, Chart.Symbol);

            var barsNumber = mainFan.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var mainFanPriceDelta = mainFan.GetPriceDelta();

            for (var iFan = 0; iFan < SideFanSettings.Length; iFan++)
            {
                var fanSettings = SideFanSettings[iFan];

                double y2;
                DateTime time2;

                if (fanSettings.Percent < 0)
                {
                    var yAmount = mainFanPriceDelta * Math.Abs(fanSettings.Percent);

                    y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 - yAmount : mainFan.Y2 + yAmount;

                    time2 = mainFan.Time2;
                }
                else
                {
                    y2 = mainFan.Y2;

                    var barsPercent = barsNumber * fanSettings.Percent;

                    var barIndex = mainFan.Time2 > mainFan.Time1 ? endBarIndex - barsPercent : startBarIndex + barsPercent;

                    time2 = Chart.Bars.GetOpenTime(barIndex, Chart.Symbol);
                }

                var objectName = GetObjectName(string.Format("SideFan_{0}", fanSettings.Percent));

                var trendLine = Chart.DrawTrendLine(objectName, mainFan.Time1, mainFan.Y1, time2, y2, fanSettings.Color, fanSettings.Thickness, fanSettings.Style);

                trendLine.IsInteractive = true;
                trendLine.IsLocked = true;
                trendLine.ExtendToInfinity = true;

                SideFanLines[fanSettings.Percent] = trendLine;
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _mainFanLine };
        }
    }
}