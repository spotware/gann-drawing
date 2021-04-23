using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace cAlgo.ChartObjectModels
{
    public static class ChartObjectsSerializer
    {
        public static void Serialize(IChartObjectModel[] chartObjectModels, string fileName)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var container = new ChartObjectsContainer
                {
                    TextModels = chartObjectModels.Where(iModel => iModel is ChartTextModel).Cast<ChartTextModel>().ToArray(),
                    TriangleModels = chartObjectModels.Where(iModel => iModel is ChartTriangleModel).Cast<ChartTriangleModel>().ToArray(),
                    VerticalLineModels = chartObjectModels.Where(iModel => iModel is ChartVerticalLineModel).Cast<ChartVerticalLineModel>().ToArray(),
                    TrendLineModels = chartObjectModels.Where(iModel => iModel is ChartTrendLineModel).Cast<ChartTrendLineModel>().ToArray(),
                    RectangleModels = chartObjectModels.Where(iModel => iModel is ChartRectangleModel).Cast<ChartRectangleModel>().ToArray(),
                };

                var serializer = new XmlSerializer(typeof(ChartObjectsContainer));

                serializer.Serialize(fileStream, container);
            }
        }

        public static IChartObjectModel[] Deserialize(string fileName)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var serializer = new XmlSerializer(typeof(ChartObjectsContainer));

                return (serializer.Deserialize(fileStream) as ChartObjectsContainer).GetAllModels();
            }
        }

        public sealed class ChartObjectsContainer
        {
            public ChartTextModel[] TextModels { get; set; }

            public ChartTriangleModel[] TriangleModels { get; set; }

            public ChartRectangleModel[] RectangleModels { get; set; }

            public ChartVerticalLineModel[] VerticalLineModels { get; set; }

            public ChartTrendLineModel[] TrendLineModels { get; set; }

            public IChartObjectModel[] GetAllModels()
            {
                var models = new List<IChartObjectModel>();

                models.AddRange(TextModels);
                models.AddRange(TriangleModels);
                models.AddRange(RectangleModels);
                models.AddRange(VerticalLineModels);
                models.AddRange(TrendLineModels);

                return models.ToArray();
            }
        }
    }
}