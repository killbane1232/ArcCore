namespace Arcam.Data.DataTypes
{
    public class IndicatorDataItem
    {
        public DateTime TimeStamp { get; set; }
        public List<double> Values { get; set; } = [];
    }
}
