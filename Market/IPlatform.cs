using Arcam.Data.DataBase.DBTypes;
using Arcam.Data.DataTypes;

namespace Arcam.Market
{
    public interface IPlatform
    {
        List<Candle> TakeCandles(string symbol, int count, Timing timing, bool reverse = false, DateTimeOffset? where = null);
        string ClosePosition(string symbol);
        string PostOrders(bool buy, double quality, string symbol);
        Dictionary<string, PositionInfo> EncountPositions();
        WalletInfo TakeWallet();
        string SetLeverage(string symbol, double leverage = 0);
        DateTime NextCheckDate();
        double TakeMidPrice(string symbol);
    }
}
