using cAlgo.API;
using System;

namespace cAlgo.ChartObjectModels
{
    public static class DrawModelsOnChart
    {
        public static ChartObject DrawModel(this Chart chart, IChartObjectModel model)
        {
            ChartObject chartObject;

            switch (model.ObjectType)
            {
                case ChartObjectType.TrendLine:
                    var trendLineModel = model as ChartTrendLineModel;

                    var chartTrendLine = chart.DrawTrendLine(trendLineModel.Name, trendLineModel.Time1, trendLineModel.Y1, trendLineModel.Time2, trendLineModel.Y2, Color.FromHex(trendLineModel.ColorHex), trendLineModel.Thickness, trendLineModel.LineStyle);

                    chartTrendLine.ShowAngle = trendLineModel.ShowAngle;
                    chartTrendLine.ExtendToInfinity = trendLineModel.ExtendToInfinity;

                    chartObject = chartTrendLine;

                    break;

                case ChartObjectType.Text:
                    var textModel = model as ChartTextModel;

                    var chartText = chart.DrawText(textModel.Name, textModel.Text, textModel.Time, textModel.Y, Color.FromHex(textModel.ColorHex));

                    chartText.HorizontalAlignment = textModel.HorizontalAlignment;
                    chartText.VerticalAlignment = textModel.VerticalAlignment;
                    chartText.FontSize = textModel.FontSize;
                    chartText.IsBold = textModel.IsBold;
                    chartText.IsItalic = textModel.IsItalic;
                    chartText.IsUnderlined = textModel.IsUnderlined;

                    chartObject = chartText;

                    break;

                case ChartObjectType.Triangle:
                    var triangleModel = model as ChartTriangleModel;

                    chartObject = chart.DrawTriangle(triangleModel.Name, triangleModel.Time1, triangleModel.Y1, triangleModel.Time2, triangleModel.Y2, triangleModel.Time3, triangleModel.Y3, Color.FromHex(triangleModel.ColorHex));

                    break;

                case ChartObjectType.Rectangle:
                    var rectangleModel = model as ChartRectangleModel;

                    chartObject = chart.DrawRectangle(rectangleModel.Name, rectangleModel.Time1, rectangleModel.Y1, rectangleModel.Time2, rectangleModel.Y2, Color.FromHex(rectangleModel.ColorHex));

                    break;

                case ChartObjectType.VerticalLine:
                    var verticalLineModel = model as ChartVerticalLineModel;

                    chartObject = chart.DrawVerticalLine(verticalLineModel.Name, verticalLineModel.Time, Color.FromHex(verticalLineModel.ColorHex), verticalLineModel.Thickness, verticalLineModel.LineStyle);

                    break;

                default:
                    throw new InvalidOperationException(string.Format("Not supported object type {0}", model.ObjectType));
            }

            if (chartObject != null)
            {
                if (chartObject is ChartShape && model is ChartShapeModel)
                {
                    var chartShape = chartObject as ChartShape;
                    var chartShapeModel = model as ChartShapeModel;

                    chartShape.IsFilled = chartShapeModel.IsFilled;
                    chartShape.LineStyle = chartShapeModel.LineStyle;
                    chartShape.Thickness = chartShapeModel.Thickness;
                    chartShape.Color = Color.FromHex(chartShapeModel.ColorHex);
                }

                chartObject.Comment = model.Comment;
                chartObject.IsInteractive = model.IsInteractive;
                chartObject.IsHidden = model.IsHidden;
                chartObject.IsLocked = model.IsLocked;
                chartObject.ZIndex = model.ZIndex;
            }

            return chartObject;
        }

        public static void DrawModels(this Chart chart, IChartObjectModel[] models)
        {
            foreach (var model in models)
            {
                chart.DrawModel(model);
            }
        }
    }
}