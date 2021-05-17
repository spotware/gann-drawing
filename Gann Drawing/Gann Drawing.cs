﻿using cAlgo.API;
using cAlgo.Controls;
using cAlgo.Helpers;
using cAlgo.Patterns;
using System.Collections.Generic;

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

        private readonly List<Button> _buttons = new List<Button>();

        private Button _expandButton;

        #region Patterns color parameters

        [Parameter("Color", DefaultValue = "Red", Group = "Patterns Color")]
        public string PatternsColor { get; set; }

        [Parameter("Alpha", DefaultValue = 100, MinValue = 0, MaxValue = 255, Group = "Patterns Color")]
        public int PatternsColorAlpha { get; set; }

        #endregion Patterns color parameters

        #region Patterns Label parameters

        [Parameter("Show", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelShow { get; set; }

        [Parameter("Color", DefaultValue = "Yellow", Group = "Patterns Label")]
        public string PatternsLabelColor { get; set; }

        [Parameter("Alpha", DefaultValue = 100, MinValue = 0, MaxValue = 255, Group = "Patterns Label")]
        public int PatternsLabelColorAlpha { get; set; }

        [Parameter("Locked", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelLocked { get; set; }

        [Parameter("Link Style", DefaultValue = true, Group = "Patterns Label")]
        public bool PatternsLabelLinkStyle { get; set; }

        #endregion Patterns Label parameters

        #region Container Panel parameters

        [Parameter("Orientation", DefaultValue = Orientation.Vertical, Group = "Container Panel")]
        public Orientation PanelOrientation { get; set; }

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Left, Group = "Container Panel")]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Top, Group = "Container Panel")]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Margin", DefaultValue = 3, Group = "Container Panel")]
        public double PanelMargin { get; set; }

        #endregion Container Panel parameters

        #region Buttons parameters

        [Parameter("Disable Color", DefaultValue = "#FFCCCCCC", Group = "Buttons")]
        public string ButtonsBackgroundDisableColor { get; set; }

        [Parameter("Enable Color", DefaultValue = "Red", Group = "Buttons")]
        public string ButtonsBackgroundEnableColor { get; set; }

        [Parameter("Text Color", DefaultValue = "Blue", Group = "Buttons")]
        public string ButtonsForegroundColor { get; set; }

        [Parameter("Margin", DefaultValue = 1, Group = "Buttons")]
        public double ButtonsMargin { get; set; }

        [Parameter("Transparency", DefaultValue = 0.5, MinValue = 0, MaxValue = 1, Group = "Buttons")]
        public double ButtonsTransparency { get; set; }

        #endregion Buttons parameters

        #region TimeFrame Visibility parameters

        [Parameter("Enable", DefaultValue = false, Group = "TimeFrame Visibility")]
        public bool IsTimeFrameVisibilityEnabled { get; set; }

        [Parameter("TimeFrame", Group = "TimeFrame Visibility")]
        public TimeFrame VisibilityTimeFrame { get; set; }

        [Parameter("Only Buttons", Group = "TimeFrame Visibility")]
        public bool VisibilityOnlyButtons { get; set; }

        #endregion TimeFrame Visibility parameters

        #region Gann Box parameters

        [Parameter("Rectangle Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Box")]
        public int GannBoxRectangleThickness { get; set; }

        [Parameter("Rectangle Style", DefaultValue = LineStyle.Solid, Group = "Gann Box")]
        public LineStyle GannBoxRectangleStyle { get; set; }

        [Parameter("Rectangle Color", DefaultValue = "Blue", Group = "Gann Box")]
        public string GannBoxRectangleColor { get; set; }

        [Parameter("Price Levels Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Box")]
        public int GannBoxPriceLevelsThickness { get; set; }

        [Parameter("Price Levels Style", DefaultValue = LineStyle.Solid, Group = "Gann Box")]
        public LineStyle GannBoxPriceLevelsStyle { get; set; }

        [Parameter("Price Levels Color", DefaultValue = "Magenta", Group = "Gann Box")]
        public string GannBoxPriceLevelsColor { get; set; }

        [Parameter("Time Levels Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Box")]
        public int GannBoxTimeLevelsThickness { get; set; }

        [Parameter("Time Levels Style", DefaultValue = LineStyle.Solid, Group = "Gann Box")]
        public LineStyle GannBoxTimeLevelsStyle { get; set; }

        [Parameter("Time Levels Color", DefaultValue = "Yellow", Group = "Gann Box")]
        public string GannBoxTimeLevelsColor { get; set; }

        #endregion Gann Box parameters

        #region Gann Square parameters

        [Parameter("Rectangle Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Square")]
        public int GannSquareRectangleThickness { get; set; }

        [Parameter("Rectangle Style", DefaultValue = LineStyle.Solid, Group = "Gann Square")]
        public LineStyle GannSquareRectangleStyle { get; set; }

        [Parameter("Rectangle Color", DefaultValue = "Blue", Group = "Gann Square")]
        public string GannSquareRectangleColor { get; set; }

        [Parameter("Price Levels Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Square")]
        public int GannSquarePriceLevelsThickness { get; set; }

        [Parameter("Price Levels Style", DefaultValue = LineStyle.Solid, Group = "Gann Square")]
        public LineStyle GannSquarePriceLevelsStyle { get; set; }

        [Parameter("Price Levels Color", DefaultValue = "Magenta", Group = "Gann Square")]
        public string GannSquarePriceLevelsColor { get; set; }

        [Parameter("Time Levels Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Square")]
        public int GannSquareTimeLevelsThickness { get; set; }

        [Parameter("Time Levels Style", DefaultValue = LineStyle.Solid, Group = "Gann Square")]
        public LineStyle GannSquareTimeLevelsStyle { get; set; }

        [Parameter("Time Levels Color", DefaultValue = "Yellow", Group = "Gann Square")]
        public string GannSquareTimeLevelsColor { get; set; }

        [Parameter("Fans Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Square")]
        public int GannSquareFansThickness { get; set; }

        [Parameter("Fans Style", DefaultValue = LineStyle.Solid, Group = "Gann Square")]
        public LineStyle GannSquareFansStyle { get; set; }

        [Parameter("Fans Color", DefaultValue = "Brown", Group = "Gann Square")]
        public string GannSquareFansColor { get; set; }

        #endregion Gann Square parameters

        #region Gann Fan parameters

        [Parameter("1/1 Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Fan")]
        public int GannFanOneThickness { get; set; }

        [Parameter("1/1 Style", DefaultValue = LineStyle.Solid, Group = "Gann Fan")]
        public LineStyle GannFanOneStyle { get; set; }

        [Parameter("1/1 Color", DefaultValue = "Red", Group = "Gann Fan")]
        public string GannFanOneColor { get; set; }

        [Parameter("1/2 and 2/1 Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Fan")]
        public int GannFanTwoThickness { get; set; }

        [Parameter("1/2 and 2/1 Style", DefaultValue = LineStyle.Solid, Group = "Gann Fan")]
        public LineStyle GannFanTwoStyle { get; set; }

        [Parameter("1/2 and 2/1 Color", DefaultValue = "Brown", Group = "Gann Fan")]
        public string GannFanTwoColor { get; set; }

        [Parameter("1/3 and 3/1 Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Fan")]
        public int GannFanThreeThickness { get; set; }

        [Parameter("1/3 and 3/1 Style", DefaultValue = LineStyle.Solid, Group = "Gann Fan")]
        public LineStyle GannFanThreeStyle { get; set; }

        [Parameter("1/3 and 3/1 Color", DefaultValue = "Lime", Group = "Gann Fan")]
        public string GannFanThreeColor { get; set; }

        [Parameter("1/4 and 4/1 Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Fan")]
        public int GannFanFourThickness { get; set; }

        [Parameter("1/4 and 4/1 Style", DefaultValue = LineStyle.Solid, Group = "Gann Fan")]
        public LineStyle GannFanFourStyle { get; set; }

        [Parameter("1/4 and 4/1 Color", DefaultValue = "Magenta", Group = "Gann Fan")]
        public string GannFanFourColor { get; set; }

        [Parameter("1/8 and 8/1 Thickness", DefaultValue = 1, MinValue = 1, Group = "Gann Fan")]
        public int GannFanEightThickness { get; set; }

        [Parameter("1/8 and 8/1 Style", DefaultValue = LineStyle.Solid, Group = "Gann Fan")]
        public LineStyle GannFanEightStyle { get; set; }

        [Parameter("1/8 and 8/1 Color", DefaultValue = "Blue", Group = "Gann Fan")]
        public string GannFanEightColor { get; set; }

        #endregion Gann Fan parameters

        #region Overridden methods

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

            var patternConfig = new PatternConfig(Chart, patternsColor, PatternsLabelShow, patternsLabelsColor, PatternsLabelLocked, PatternsLabelLinkStyle, new Logger(this.GetType().Name, Print));

            _expandButton = new Button
            {
                Style = _buttonsStyle,
                Text = "Expand Patterns"
            };

            _expandButton.Click += ExpandButton_Click;

            _mainButtonsPanel.AddChild(_expandButton);

            AddPatternButton(new GannBoxPattern(patternConfig, new GannBoxSettings
            {
                RectangleThickness = GannBoxRectangleThickness,
                RectangleStyle = GannBoxRectangleStyle,
                RectangleColor = ColorParser.Parse(GannBoxRectangleColor),
                PriceLevelsThickness = GannBoxPriceLevelsThickness,
                PriceLevelsStyle = GannBoxPriceLevelsStyle,
                PriceLevelsColor = ColorParser.Parse(GannBoxPriceLevelsColor),
                TimeLevelsThickness = GannBoxTimeLevelsThickness,
                TimeLevelsStyle = GannBoxTimeLevelsStyle,
                TimeLevelsColor = ColorParser.Parse(GannBoxTimeLevelsColor),
            }));

            AddPatternButton(new GannSquarePattern(patternConfig, new GannSquareSettings
            {
                RectangleThickness = GannSquareRectangleThickness,
                RectangleStyle = GannSquareRectangleStyle,
                RectangleColor = ColorParser.Parse(GannSquareRectangleColor),
                PriceLevelsThickness = GannSquarePriceLevelsThickness,
                PriceLevelsStyle = GannSquarePriceLevelsStyle,
                PriceLevelsColor = ColorParser.Parse(GannSquarePriceLevelsColor),
                TimeLevelsThickness = GannSquareTimeLevelsThickness,
                TimeLevelsStyle = GannSquareTimeLevelsStyle,
                TimeLevelsColor = ColorParser.Parse(GannSquareTimeLevelsColor),
                FansThickness = GannSquareFansThickness,
                FansStyle = GannSquareFansStyle,
                FansColor = ColorParser.Parse(GannSquareFansColor),
            }));

            AddPatternButton(new GannFanPattern(patternConfig, new SideFanSettings[]
            {
                new SideFanSettings
                {
                    Name = "1x2",
                    Percent = 0.416,
                    Color = ColorParser.Parse(GannFanTwoColor),
                    Style = GannFanTwoStyle,
                    Thickness = GannFanTwoThickness
                },
                new SideFanSettings
                {
                    Name = "1x3",
                    Percent = 0.583,
                    Color = ColorParser.Parse(GannFanThreeColor),
                    Style = GannFanThreeStyle,
                    Thickness = GannFanThreeThickness
                },
                new SideFanSettings
                {
                    Name = "1x4",
                    Percent = 0.666,
                    Color = ColorParser.Parse(GannFanFourColor),
                    Style = GannFanFourStyle,
                    Thickness = GannFanFourThickness
                },
                new SideFanSettings
                {
                    Name = "1x8",
                    Percent = 0.833,
                    Color = ColorParser.Parse(GannFanEightColor),
                    Style = GannFanEightStyle,
                    Thickness = GannFanEightThickness
                },
                new SideFanSettings
                {
                    Name = "2x1",
                    Percent = -0.416,
                    Color = ColorParser.Parse(GannFanTwoColor),
                    Style = GannFanTwoStyle,
                    Thickness = GannFanTwoThickness
                },
                new SideFanSettings
                {
                    Name = "3x1",
                    Percent = -0.583,
                    Color = ColorParser.Parse(GannFanThreeColor),
                    Style = GannFanThreeStyle,
                    Thickness = GannFanThreeThickness
                },
                new SideFanSettings
                {
                    Name = "4x1",
                    Percent = -0.666,
                    Color = ColorParser.Parse(GannFanFourColor),
                    Style = GannFanFourStyle,
                    Thickness = GannFanFourThickness
                },
                new SideFanSettings
                {
                    Name = "8x1",
                    Percent = -0.833,
                    Color = ColorParser.Parse(GannFanEightColor),
                    Style = GannFanEightStyle,
                    Thickness = GannFanEightThickness
                },
                }, new FanSettings
                {
                    Color = ColorParser.Parse(GannFanOneColor),
                    Style = GannFanOneStyle,
                    Thickness = GannFanOneThickness
                }));

            var showHideButton = new Controls.ToggleButton()
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor,
                Text = "Hide",
                IsVisible = false
            };

            showHideButton.TurnedOn += ShowHideButton_TurnedOn;
            showHideButton.TurnedOff += ShowHideButton_TurnedOff;

            _mainButtonsPanel.AddChild(showHideButton);
            _buttons.Add(showHideButton);

            var saveButton = new PatternsSaveButton(Chart)
            {
                Style = _buttonsStyle,
                IsVisible = false
            };

            _mainButtonsPanel.AddChild(saveButton);
            _buttons.Add(saveButton);

            var loadButton = new PatternsLoadButton(Chart)
            {
                Style = _buttonsStyle,
                IsVisible = false
            };

            _mainButtonsPanel.AddChild(loadButton);
            _buttons.Add(loadButton);

            var removeAllButton = new PatternsRemoveAllButton(Chart)
            {
                Style = _buttonsStyle,
                IsVisible = false
            };

            _mainButtonsPanel.AddChild(removeAllButton);
            _buttons.Add(removeAllButton);

            var collapseButton = new Button
            {
                Style = _buttonsStyle,
                Text = "Collapse",
                IsVisible = false
            };

            collapseButton.Click += CollapseButton_Click;

            _mainButtonsPanel.AddChild(collapseButton);
            _buttons.Add(collapseButton);

            Chart.AddControl(_mainPanel);

            CheckTimeFrameVisibility();
        }

        public override void Calculate(int index)
        {
        }

        #endregion Overridden methods

        private void CollapseButton_Click(ButtonClickEventArgs obj)
        {
            _buttons.ForEach(iButton => iButton.IsVisible = false);

            _groupButtonsPanel.IsVisible = false;

            _expandButton.IsVisible = true;
        }

        private void ExpandButton_Click(ButtonClickEventArgs obj)
        {
            _buttons.ForEach(iButton => iButton.IsVisible = true);

            obj.Button.IsVisible = false;
        }

        private void ShowHideButton_TurnedOff(Controls.ToggleButton obj)
        {
            Chart.ChangePatternsVisibility(false);

            obj.Text = "Hide";
        }

        private void ShowHideButton_TurnedOn(Controls.ToggleButton obj)
        {
            Chart.ChangePatternsVisibility(true);

            obj.Text = "Show";
        }

        private void AddPatternButton(IPattern pattern)
        {
            var button = new PatternButton(pattern)
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor,
                IsVisible = false
            };

            _buttons.Add(button);

            _mainButtonsPanel.AddChild(button);

            pattern.Initialize();
        }

        private void CheckTimeFrameVisibility()
        {
            if (IsTimeFrameVisibilityEnabled)
            {
                if (TimeFrame != VisibilityTimeFrame)
                {
                    _mainButtonsPanel.IsVisible = false;

                    if (!VisibilityOnlyButtons) Chart.ChangePatternsVisibility(true);
                }
                else if (!VisibilityOnlyButtons)
                {
                    Chart.ChangePatternsVisibility(false);
                }
            }
        }
    }
}