using cAlgo.API;
using cAlgo.Controls;
using cAlgo.Helpers;
using cAlgo.Patterns;
using System.Linq;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class GannDrawing : Indicator
    {
        private StackPanel _mainButtonsPanel;

        private StackPanel _groupButtonsPanel;

        private StackPanel _mainPanel;

        private Color _buttonsBackgroundDisableColor;

        private Color _buttonsBackgroundEnableColor;

        private Style _buttonsStyle;

        [Parameter("Color", DefaultValue = "Red", Group = "Patterns Color")]
        public string PatternsColor { get; set; }

        [Parameter("Alpha", DefaultValue = 100, MinValue = 0, MaxValue = 255, Group = "Patterns Color")]
        public int PatternsColorAlpha { get; set; }

        [Parameter("Show", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelShow { get; set; }

        [Parameter("Color", DefaultValue = "Yellow", Group = "Patterns Label")]
        public string PatternsLabelColor { get; set; }

        [Parameter("Alpha", DefaultValue = 100, MinValue = 0, MaxValue = 255, Group = "Patterns Label")]
        public int PatternsLabelColorAlpha { get; set; }

        [Parameter("Interactive", DefaultValue = "Red", Group = "Patterns Label")]
        public bool PatternsLabelInteractive { get; set; }

        [Parameter("Orientation", DefaultValue = Orientation.Vertical, Group = "Container Panel")]
        public Orientation PanelOrientation { get; set; }

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Left, Group = "Container Panel")]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Top, Group = "Container Panel")]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Margin", DefaultValue = 3, Group = "Container Panel")]
        public double PanelMargin { get; set; }

        [Parameter("Disable Color", DefaultValue = "#FFCCCCCC", Group = "Buttons")]
        public string ButtonsBackgroundDisableColor { get; set; }

        [Parameter("Enable Color", DefaultValue = "Red", Group = "Buttons")]
        public string ButtonsBackgroundEnableColor { get; set; }

        [Parameter("Text Color", DefaultValue = "Blue", Group = "Buttons")]
        public string ButtonsForegroundColor { get; set; }

        [Parameter("Margin", DefaultValue = 1, Group = "Buttons")]
        public double ButtonsMargin { get; set; }

        [Parameter("Transparency", DefaultValue = 1, MinValue = 0, MaxValue = 1, Group = "Buttons")]
        public double ButtonsTransparency { get; set; }

        [Parameter("Enable", DefaultValue = false, Group = "TimeFrame Visibility")]
        public bool IsTimeFrameVisibilityEnabled { get; set; }

        [Parameter("TimeFrame", Group = "TimeFrame Visibility")]
        public TimeFrame VisibilityTimeFrame { get; set; }

        [Parameter("Only Buttons", Group = "TimeFrame Visibility")]
        public bool VisibilityOnlyButtons { get; set; }

        protected override void Initialize()
        {
            _mainPanel = new StackPanel
            {
                HorizontalAlignment = PanelHorizontalAlignment,
                VerticalAlignment = PanelVerticalAlignment,
                Orientation = PanelOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal,
                BackgroundColor = Color.Transparent,
            };

            _mainButtonsPanel = new StackPanel
            {
                Orientation = PanelOrientation,
                Margin = PanelMargin
            };

            _mainPanel.AddChild(_mainButtonsPanel);

            _groupButtonsPanel = new StackPanel
            {
                Orientation = PanelOrientation,
                Margin = PanelMargin,
                IsVisible = false
            };

            _mainPanel.AddChild(_groupButtonsPanel);

            _buttonsBackgroundDisableColor = ColorParser.Parse(ButtonsBackgroundDisableColor);
            _buttonsBackgroundEnableColor = ColorParser.Parse(ButtonsBackgroundEnableColor);

            _buttonsStyle = new Style();

            _buttonsStyle.Set(ControlProperty.Margin, ButtonsMargin);
            _buttonsStyle.Set(ControlProperty.BackgroundColor, _buttonsBackgroundDisableColor);
            _buttonsStyle.Set(ControlProperty.ForegroundColor, ColorParser.Parse(ButtonsForegroundColor));
            _buttonsStyle.Set(ControlProperty.HorizontalContentAlignment, HorizontalAlignment.Center);
            _buttonsStyle.Set(ControlProperty.VerticalContentAlignment, VerticalAlignment.Center);
            _buttonsStyle.Set(ControlProperty.Opacity, ButtonsTransparency);

            var patternsColor = ColorParser.Parse(PatternsColor, PatternsColorAlpha);
            var patternsLabelsColor = ColorParser.Parse(PatternsLabelColor, PatternsLabelColorAlpha);

            var patternConfig = new PatternConfig(Chart, patternsColor, PatternsLabelShow, patternsLabelsColor, PatternsLabelInteractive)
            {
                Print = Print
            };

            AddPatternButton(new GannBoxPattern(patternConfig));
            AddPatternButton(new GannSquarePattern(patternConfig));
            AddPatternButton(new GannFanPattern(patternConfig));

            var showHideButton = new Controls.ToggleButton()
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor,
                Text = "Hide"
            };

            showHideButton.TurnedOn += ShowHideButton_TurnedOn;
            showHideButton.TurnedOff += ShowHideButton_TurnedOff;

            _mainButtonsPanel.AddChild(showHideButton);

            _mainButtonsPanel.AddChild(new PatternsSaveButton(Chart)
            {
                Style = _buttonsStyle
            });

            _mainButtonsPanel.AddChild(new PatternsLoadButton(Chart)
            {
                Style = _buttonsStyle
            });

            _mainButtonsPanel.AddChild(new PatternsRemoveAllButton(Chart)
            {
                Style = _buttonsStyle
            });

            Chart.AddControl(_mainPanel);

            CheckTimeFrameVisibility();
        }

        public override void Calculate(int index)
        {
        }

        private void ShowHideButton_TurnedOff(Controls.ToggleButton obj)
        {
            ChangePatternsVisibility(false);

            obj.Text = "Hide";
        }

        private void ShowHideButton_TurnedOn(Controls.ToggleButton obj)
        {
            ChangePatternsVisibility(true);

            obj.Text = "Show";
        }

        private void AddPatternButton(IPattern pattern)
        {
            _mainButtonsPanel.AddChild(new PatternButton(pattern)
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor
            });

            pattern.Initialize();
        }

        private void ChangePatternsVisibility(bool isHidden)
        {
            var chartObjects = Chart.Objects.ToArray();

            foreach (var chartObject in chartObjects)
            {
                if (!chartObject.IsPattern()) continue;

                chartObject.IsHidden = isHidden;
            }
        }

        private void CheckTimeFrameVisibility()
        {
            if (IsTimeFrameVisibilityEnabled)
            {
                if (TimeFrame != VisibilityTimeFrame)
                {
                    _mainButtonsPanel.IsVisible = false;

                    if (!VisibilityOnlyButtons) ChangePatternsVisibility(true);
                }
                else if (!VisibilityOnlyButtons)
                {
                    ChangePatternsVisibility(false);
                }
            }
        }
    }
}