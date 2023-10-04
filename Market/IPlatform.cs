using Arcam.Data.DataTypes;

namespace Arcam.Market
{
    public interface IPlatform
    {
        string GetPositions();
        List<Candle> TakeCandles(string symbol, int count, int timeSpan, bool reverse = false, DateTimeOffset? where = null);
        string ClosePosition(string symbol);
        string PostOrders(bool buy, int quality, string symbol);
        Dictionary<string, PositionInfo> EncountPositions();
        WalletInfo TakeWallet();
        string SetLeverage(string symbol, double leverage = 0);
        DateTime NextCheckDate();
        double TakeMidPrice(string symbol);
    }
}
