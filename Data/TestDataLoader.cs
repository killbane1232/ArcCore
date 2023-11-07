using Arcam.Data.DataBase.DBTypes;
using Arcam.Data.DataTypes;
using Arcam.Market;
using System.Diagnostics;

namespace Arcam.Data
{
    public class TestDataLoader
    {
        IPlatform platform;
        string symbol;
        public TestDataLoader(Account account)
        {
            platform = (IPlatform)Type.GetType(account.Platform.ClassName)!.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(string) })!.Invoke(new object[] { account.Platform.Url, account.Key, account.Secret });
            symbol = account.Strategy.Pair.Name;
        }


        public List<Candle> GetData()
        {
            var fileName = $"{symbol}TestData.tac";
            var serializer = new DataSerializer<Candle>(fileName);
            serializer.LoadData();
            var candles = serializer.Data;
            if (candles == null || candles.Count == 0)
            {
                candles = DownloadData();
                serializer.Data = candles;
                serializer.SaveData();
            }
            return candles;
        }

        private List<Candle> DownloadData()
        {
            var candles = new List<Candle>();
            var time = new DateTimeOffset(new DateTime(2019, 1, 1));
            time = time.AddMinutes(480);
            var cnt = (long)Math.Floor((DateTimeOffset.UtcNow - time).TotalMinutes);
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (time.AddMinutes(480) < DateTimeOffset.UtcNow)
            {
                Console.SetCursorPosition(0, 0);
                if (candles.Count / (double)cnt * 100f > 97)
                    Console.WriteLine();
                var percentage = candles.Count / (double)cnt * 100f;
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
                Console.WriteLine($"] {percentage.ToString("00.00")}% ");
                var timeToWin = new DateTime();

                if (candles.Count>0)
                    timeToWin = timeToWin.AddMilliseconds(stopwatch.ElapsedMilliseconds * ((cnt - candles.Count) / candles.Count ));
                Console.WriteLine(timeToWin + " Left  ");

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
