using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.ChartObjectModels
{
    public static class ChartObjectToModel
    {
        public static IChartObjectModel GetObjectModel(this ChartObject chartObject)
        {
            IChartObjectModel chartObjectModel;

            switch (chartObject.ObjectType)
            {
                case ChartObjectType.TrendLine:
                    var chartTrendLine = chartObject as ChartTrendLine;

                    chartObjectModel = new ChartTrendLineModel
                    {
                        Time1 = chartTrendLine.Time1,
                        Time2 = chartTrendLine.Time2,
                        Y1 = chartTrendLine.Y1,
                        Y2 = chartTrendLine.Y2,
                        ShowAngle = chartTrendLine.ShowAngle,
                        ColorHex = chartTrendLine.Color.ToHexString(),
                        ExtendToInfinity = chartTrendLine.ExtendToInfinity,
                        LineStyle = chartTrendLine.LineStyle,
                        Thickness = chartTrendLine.Thickness
                    };

                    break;

                case ChartObjectType.Text:
                    var chartText = chartObject as ChartText;

                    chartObjectModel = new ChartTextModel
                    {
                        Time = chartText.Time,
                        Y = chartText.Y,
                        ColorHex = chartText.Color.ToHexString(),
                        Text = chartText.Text,
                        HorizontalAlignment = chartText.HorizontalAlignment,
                        VerticalAlignment = chartText.VerticalAlignment,
                        FontSize = chartText.FontSize,
                        IsBold = chartText.IsBold,
                        IsItalic = chartText.IsItalic,
                        IsUnderlined = chartText.IsUnderlined
                    };

                    break;

                case ChartObjectType.Triangle:
                    var chartTriangle = chartObject as ChartTriangle;

                    chartObjectModel = new ChartTriangleModel
                    {
                        Time1 = chartTriangle.Time1,
                        Time2 = chartTriangle.Time2,
                        Time3 = chartTriangle.Time3,
                        Y1 = chartTriangle.Y1,
                        Y2 = chartTriangle.Y2,
                        Y3 = chartTriangle.Y3,
                    };

                    break;

                case ChartObjectType.Rectangle:
                    var chartRectangle = chartObject as ChartRectangle;

                    chartObjectModel = new ChartRectangleModel
                    {
                        Time1 = chartRectangle.Time1,
                        Time2 = chartRectangle.Time2,
                        Y1 = chartRectangle.Y1,
                        Y2 = chartRectangle.Y2,
                    };

                    break;

                case ChartObjectType.VerticalLine:
                    var chartVerticalLine = chartObject as ChartVerticalLine;

                    chartObjectModel = new ChartVerticalLineModel
                    {
                        Time = chartVerticalLine.Time,
                        Thickness = chartVerticalLine.Thickness,
                        ColorHex = chartVerticalLine.Color.ToHexString(),
                        LineStyle = chartVerticalLine.LineStyle
                    };

                    break;

                default:
                    throw new InvalidOperationException(string.Format("Not supported object type {0}", chartObject.ObjectType));
            }

            if (chartObjectModel != null)
            {
                if (chartObject is ChartShape && chartObjectModel is ChartShapeModel)
                {
                    var chartShape = chartObject as ChartShape;
                    var chartShapeModel = chartObjectModel as ChartShapeModel;

                    chartShapeModel.IsFilled = chartShape.IsFilled;
                    chartShapeModel.LineStyle = chartShape.LineStyle;
                    chartShapeModel.Thickness = chartShape.Thickness;
                    chartShapeModel.ColorHex = chartShape.Color.ToHexString();
                }

                chartObjectModel.Name = chartObject.Name;
                chartObjectModel.Comment = chartObject.Comment;
                chartObjectModel.IsInteractive = chartObject.IsInteractive;
                chartObjectModel.IsHidden = chartObject.IsHidden;
                chartObjectModel.IsLocked = chartObject.IsLocked;
                chartObjectModel.ZIndex = chartObject.ZIndex;
                chartObjectModel.ObjectType = chartObject.ObjectType;
            }

            return chartObjectModel;
        }

        public static IChartObjectModel[] GetObjectModels(this Chart chart)
        {
            var chartObjects = chart.Objects.ToArray();

            var chartObjectModels = new List<IChartObjectModel>();

            foreach (var chartObject in chartObjects)
            {
                if (!chartObject.IsPattern()) continue;

                var objectModel = chartObject.GetObjectModel();

                chartObjectModels.Add(objectModel);
            }

            return chartObjectModels.ToArray();
        }
    }
}