using Arcam.Data.DataTypes;
using Arcam.Market;

namespace Arcam.Data
{
    public class DataLoader
    {
        IPlatform platform;
        public DataLoader(IPlatform platform)
        {
            this.platform = platform;
        }


        public List<Candle> GetData(string symbol)
        {
            var fileName = $"{symbol}TestData.tac";
            var serializer = new DataSerializer<Candle>(fileName);
            serializer.LoadData();
            var candles = serializer.Data;
            if (candles == null || candles.Count == 0)
            {
                candles = DownloadData(symbol);
                serializer.Data = candles;
                serializer.SaveData();
            }
            return candles;
        }

        private List<Candle> DownloadData(string symbol)
        {
            var candles = new List<Candle>();
            var time = new DateTimeOffset(new DateTime(2019, 1, 1));
            time = time.AddMinutes(480);
            var cnt = (DateTimeOffset.UtcNow - time).TotalMinutes;
            while (time.AddMinutes(480) < DateTimeOffset.UtcNow)
            {
                Console.SetCursorPosition(0, 0);
                if (candles.Count / (double)cnt * 100f > 97)
                    Console.WriteLine();
                var percentage = Math.Round(candles.Count / (double)cnt * 100f, 2);
                int perCnt = (int)percentage / 2;
                Console.Write('[');
                for (int i = 0; i < perCnt; i++)
                {
                    Console.Write('#');
                }
                for (int i = perCnt; i < 50; i++)
                {
                    Console.Write(' ');
                }
                Console.WriteLine($"] {percentage}% ");

                var range = platform.TakeCandles(symbol, 480, -2, where: time);
                while (range.Count != 480)
                {
                    Thread.Sleep(500);
                    range = platform.TakeCandles(symbol, 480, -2, where: time);
                }
                candles.AddRange(range);
                time = time.AddMinutes(480);
                Thread.Sleep(500);
            }

            return candles;
        }
    }
}
