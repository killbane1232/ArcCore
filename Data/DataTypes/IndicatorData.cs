namespace Arcam.Data.DataTypes
{
    public class IndicatorData
    {
        public string Name { get; set; } = "";
        public List<IndicatorDataItem> Values { get; set; } = [];
        public List<string>? LineNames { get; set; } = null;
        public string Type { get; set; } = "";
        public bool IsOnChart = false;
    }
}
