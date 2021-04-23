using cAlgo.API;

namespace cAlgo.ChartObjectModels
{
    public abstract class ChartObjectBaseModel : IChartObjectModel
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public bool IsInteractive { get; set; }

        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }

        public int ZIndex { get; set; }
        public ChartObjectType ObjectType { get; set; }
    }
}