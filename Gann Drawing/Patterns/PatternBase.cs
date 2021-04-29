using cAlgo.API;
using System;
using System.Globalization;
using System.Linq;

namespace cAlgo.Patterns
{
    public abstract class PatternBase : IPattern
    {
        private int _mouseUpNumber;
        private bool _isMouseDown;
        private bool _isDrawing;

        public PatternBase(string name, PatternConfig config, string objectName = null)
        {
            Name = name;
            Config = config;

            ObjectName = string.IsNullOrWhiteSpace(objectName) ? string.Format("Pattern_{0}", Name.Replace(" ", "").Replace("_", "")) : objectName;
        }

        protected PatternConfig Config { get; private set; }

        protected Chart Chart
        {
            get { return Config.Chart; }
        }

        protected int MouseUpNumber
        {
            get { return _mouseUpNumber; }
        }

        protected bool IsMouseDown
        {
            get { return _isMouseDown; }
        }

        protected Color Color
        {
            get { return Config.Color; }
        }

        protected Color LabelsColor
        {
            get { return Config.LabelsColor; }
        }

        protected bool ShowLabels
        {
            get { return Config.ShowLabels; }
        }

        public string Name { get; private set; }

        public bool IsDrawing
        {
            get { return _isDrawing; }
        }

        public string ObjectName { get; private set; }

        protected long Id { get; private set; }

        public event Action<IPattern> DrawingStarted;

        public event Action<IPattern> DrawingStopped;

        protected virtual ChartObject MainObject { get { return null; } }

        public void Initialize()
        {
            OnInitialize();

            ReloadPatterns(Chart.Objects.ToArray());

            Config.Chart.ObjectsRemoved += Chart_ObjectsRemoved;
            Config.Chart.ObjectsUpdated += Chart_ObjectsUpdated;

            OnInitialized();
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnInitialized()
        {
        }

        public void StartDrawing()
        {
            if (IsDrawing) return;

            _isDrawing = true;

            Id = DateTime.Now.Ticks;

            Chart.MouseDown += Chart_MouseDown;
            Chart.MouseMove += Chart_MouseMove;
            Chart.MouseUp += Chart_MouseUp;

            Chart.IsScrollingEnabled = false;

            OnDrawingStarted();

            var drawingStarted = DrawingStarted;

            if (drawingStarted != null)
            {
                drawingStarted.Invoke(this);
            }
        }

        public void StopDrawing()
        {
            if (!IsDrawing) return;

            _isDrawing = false;

            if (ShowLabels) DrawLabels();

            Chart.MouseDown -= Chart_MouseDown;
            Chart.MouseMove -= Chart_MouseMove;
            Chart.MouseUp -= Chart_MouseUp;

            Chart.IsScrollingEnabled = true;

            _mouseUpNumber = 0;

            Id = 0;

            OnDrawingStopped();

            var drawingStopped = DrawingStopped;

            if (drawingStopped != null)
            {
                drawingStopped.Invoke(this);
            }
        }

        protected void FinishDrawing()
        {
            SetMainObjectZIndex();

            StopDrawing();
        }

        private void SetMainObjectZIndex()
        {
            var mainObject = MainObject;

            if (mainObject != null) mainObject.ZIndex = Chart.Objects.Count - 1;
        }

        protected virtual void OnDrawingStopped()
        {
        }

        protected virtual void OnDrawingStarted()
        {
        }

        private void Chart_MouseMove(ChartMouseEventArgs obj)
        {
            OnMouseMove(obj);
        }

        private void Chart_MouseDown(ChartMouseEventArgs obj)
        {
            _isMouseDown = true;

            OnMouseDown(obj);
        }

        private void Chart_MouseUp(ChartMouseEventArgs obj)
        {
            _isMouseDown = false;

            _mouseUpNumber++;

            OnMouseUp(obj);
        }

        private void Chart_ObjectsRemoved(ChartObjectsRemovedEventArgs obj)
        {
            var removedPatternObjects = obj.ChartObjects.Where(iRemovedObject => iRemovedObject.Name.StartsWith(ObjectName,
                StringComparison.OrdinalIgnoreCase)).ToArray();

            if (removedPatternObjects.Length == 0) return;

            Chart.ObjectsRemoved -= Chart_ObjectsRemoved;

            try
            {
                foreach (var chartObject in removedPatternObjects)
                {
                    if (chartObject.ObjectType == ChartObjectType.Text) continue;

                    long id;

                    if (!TryGetChartObjectPatternId(chartObject.Name, out id))
                    {
                        continue;
                    }

                    RemoveObjects(id);
                }
            }
            finally
            {
                Chart.ObjectsRemoved += Chart_ObjectsRemoved;
            }
        }

        private void Chart_ObjectsUpdated(ChartObjectsUpdatedEventArgs obj)
        {
            if (IsDrawing) return;

            Chart.ObjectsUpdated -= Chart_ObjectsUpdated;

            try
            {
                ReloadPatterns(obj.ChartObjects.ToArray());
            }
            finally
            {
                Chart.ObjectsUpdated += Chart_ObjectsUpdated;
            }
        }

        private void RemoveObjects(long id)
        {
            var patternObjectNames = string.Format("{0}_{1}", ObjectName, id);

            var chartObjects = Chart.Objects.ToArray();

            foreach (var chartObject in chartObjects)
            {
                if (chartObject.Name.StartsWith(patternObjectNames, StringComparison.OrdinalIgnoreCase))
                {
                    Chart.RemoveObject(chartObject.Name);
                }
            }
        }

        protected virtual void OnMouseMove(ChartMouseEventArgs obj)
        {
        }

        protected virtual void OnMouseDown(ChartMouseEventArgs obj)
        {
        }

        protected virtual void OnMouseUp(ChartMouseEventArgs obj)
        {
        }

        protected bool TryGetChartObjectPatternId(string chartObjectName, out long id)
        {
            var objectNameSplit = chartObjectName.Split('_');

            if (objectNameSplit.Length < 3
                || !long.TryParse(objectNameSplit[2], NumberStyles.Any, CultureInfo.InvariantCulture, out id))
            {
                id = 0;

                return false;
            }

            return true;
        }

        protected virtual void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
        }

        protected virtual void DrawLabels()
        {
        }

        protected virtual void UpdateLabels(long id, ChartObject updatedObject, ChartText[] labels, ChartObject[] patternObjects)
        {
        }

        protected ChartText DrawLabelText(string text, DateTime time, double y, long id, bool isBold = false, double fontSize = default(double), string objectNameKey = null)
        {
            var name = string.IsNullOrWhiteSpace(objectNameKey)
                ? string.Format("{0}_{1}_Label_{2}", ObjectName, id, text)
                : string.Format("{0}_{1}_Label_{2}", ObjectName, id, objectNameKey);

            var chartText = Chart.DrawText(name, text, time, y, LabelsColor);

            chartText.IsInteractive = true;
            chartText.IsLocked = Config.IsLabelsLocked;
            chartText.IsBold = isBold;

            if (fontSize != default(double)) chartText.FontSize = fontSize;

            return chartText;
        }

        protected string GetObjectName(string data = null, long? id = null)
        {
            data = data ?? string.Empty;

            return string.Format("{0}_{1}_{2}", ObjectName, id.GetValueOrDefault(Id), data);
        }

        public void ReloadPatterns(ChartObject[] chartObjects)
        {
            var updatedPatternObjects = chartObjects.Where(iObject => iObject.Name.StartsWith(ObjectName,
                StringComparison.OrdinalIgnoreCase)).ToArray();

            if (updatedPatternObjects.Length == 0) return;

            foreach (var chartObject in updatedPatternObjects)
            {
                if (chartObject is ChartText) continue;

                long id;

                if (!TryGetChartObjectPatternId(chartObject.Name, out id))
                {
                    continue;
                }

                var updatedPatternName = string.Format("{0}_{1}", ObjectName, id);

                var patternObjects = Chart.Objects.Where(iObject => iObject.Name.StartsWith(updatedPatternName,
                    StringComparison.OrdinalIgnoreCase) && iObject.ObjectType != ChartObjectType.Text)
                    .ToArray();

                OnPatternChartObjectsUpdated(id, chartObject, patternObjects);

                if (ShowLabels && !patternObjects.All(iObject => iObject.IsHidden))
                {
                    var labelObjects = Chart.Objects.Where(iObject => iObject.Name.StartsWith(updatedPatternName,
                        StringComparison.OrdinalIgnoreCase) && iObject is ChartText)
                        .Select(iObject => iObject as ChartText)
                        .ToArray();

                    UpdateLabels(id, chartObject, labelObjects, patternObjects);
                }
            }
        }
    }
}