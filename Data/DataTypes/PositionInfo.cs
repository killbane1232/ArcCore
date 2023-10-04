namespace Arcam.Data.DataTypes
{
    public class PositionInfo
    {
        public bool InPosition = false;
        public bool IsLong = false;
        public int Quantity = 0;
        public double Price = 0;
        public DateTime OpenDate = DateTime.Today;
        public string Symbol = "";
    }
}
