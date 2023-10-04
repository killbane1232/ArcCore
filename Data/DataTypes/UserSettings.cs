namespace Arcam.Data.DataTypes
{
    public class UserSettings
    {
        public string ApiKey = "";
        public string ApiSecret = "";
        public Dictionary<string, int> Leverage = new Dictionary<string, int>();
    }
}
