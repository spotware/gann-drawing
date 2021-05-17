using cAlgo.API;
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
            var mainFanPriceDelta = Math.Abs(mainFan.Y2 - mainFan.Y1);

            for (var iFan = 0; iFan < _sideFanSettings.Length; iFan++)
            {
                var fanSettings = _sideFanSettings[iFan];

                var yAmount = mainFanPriceDelta * fanSettings.Percent;

                var y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 + yAmount : mainFan.Y2 - yAmount;

                ChartTrendLine fanLine;

                if (!sideFans.TryGetValue(fanSettings.Percent, out fanLine)) continue;

                fanLine.Time1 = mainFan.Time1;
                fanLine.Time2 = mainFan.Time2;

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
                FinishDrawing();

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
            if (MouseUpNumber > 1 || _mainFanLine == null) return;

            _mainFanLine.Time2 = obj.TimeValue;
            _mainFanLine.Y2 = obj.YValue;

            DrawSideFans(_mainFanLine);
        }

        protected virtual void DrawSideFans(ChartTrendLine mainFan)
        {
            var mainFanPriceDelta = Math.Abs(mainFan.Y2 - mainFan.Y1);

            for (var iFan = 0; iFan < _sideFanSettings.Length; iFan++)
            {
                var fanSettings = _sideFanSettings[iFan];

                var yAmount = mainFanPriceDelta * fanSettings.Percent;

                var y2 = mainFan.Y2 > mainFan.Y1 ? mainFan.Y2 + yAmount : mainFan.Y2 - yAmount;

                var objectName = GetObjectName(string.Format("SideFan_{0}", fanSettings.Percent));

                var trendLine = Chart.DrawTrendLine(objectName, mainFan.Time1, mainFan.Y1, mainFan.Time2, y2, fanSettings.Color, fanSettings.Thickness, fanSettings.Style);

                trendLine.IsInteractive = true;
                trendLine.IsLocked = true;
                trendLine.ExtendToInfinity = true;

                _sideFanLines[fanSettings.Percent] = trendLine;
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _mainFanLine };
        }
    }
}