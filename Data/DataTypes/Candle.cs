using Newtonsoft.Json;

namespace Arcam.Data.DataTypes
{
    public class Candle
    {
        [JsonProperty("high")]
        public double High;
        [JsonProperty("low")]
        public double Low;
        [JsonProperty("open")]
        public double Open;
        [JsonProperty("close")]
        public double Close;
        [JsonProperty("volume")]
        public double Volume;
        [JsonProperty("timestamp")]
        public DateTime TimeStamp;
    }
}
