using cAlgo.API;

namespace cAlgo.ChartObjectModels
{
    public interface IChartObjectModel
    {
        string Name { get; set; }

        string Comment { get; set; }

        bool IsInteractive { get; set; }

        bool IsLocked { get; set; }
        bool IsHidden { get; set; }

        int ZIndex { get; set; }

        ChartObjectType ObjectType { get; set; }
    }
}