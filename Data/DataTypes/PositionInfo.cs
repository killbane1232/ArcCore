namespace Arcam.Data.DataTypes
{
    public class PositionInfo
    {
        public bool InPosition = false;
        public bool IsLong = false;
        public double Quantity = 0;
        public double Price = 0;
        public DateTime OpenDate = DateTime.UtcNow;
        public string Symbol = "";
    }
}
