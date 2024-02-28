using Arcam.Data.DataBase.DBTypes;
using Newtonsoft.Json;

namespace Arcam.Data.DataTypes
{
    public class Candle
    {
        [JsonProperty("high")]
        public double High { get; set; }
        [JsonProperty("low")]
        public double Low { get; set; }
        [JsonProperty("open")]
        public double Open { get; set; }
        [JsonProperty("close")]
        public double Close { get; set; }
        [JsonProperty("volume")]
        public double Volume { get; set; }
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
        public Candle() { }
        public Candle(TradingHistory history)
        {
            High = history.High; 
            Low = history.Low;
            Open = history.Open;
            Close = history.Close;
            Volume = history.Volume;
            TimeStamp = history.TimeStamp;
        }
    }
}
