using Arcam.Data.DataTypes;
using Newtonsoft.Json;

namespace Arcam.Data
{
    public static class PositionSerializer
    {
        public static void SavePositions(Dictionary<string, PositionInfo> positions)
        {
            if (!Directory.Exists("./save"))
                _ = Directory.CreateDirectory("./save");

            var fileName = $"./save/{Thread.CurrentThread.Name}pos.txt";

            File.WriteAllText(fileName, JsonConvert.SerializeObject(positions));
        }

        public static Dictionary<string, PositionInfo> ReadPositions()
        {
            var fileName = $"./save/{Thread.CurrentThread.Name}pos.txt";
            _ = new Dictionary<string, PositionInfo>();
            Dictionary<string, PositionInfo> positions = new Dictionary<string, PositionInfo>();
            if (File.Exists(fileName))
                positions = JsonConvert.DeserializeObject<Dictionary<string, PositionInfo>>(File.ReadAllText(fileName)) ?? positions;

            return positions;
        }
    }
}
