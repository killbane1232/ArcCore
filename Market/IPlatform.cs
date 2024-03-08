﻿using Arcam.Data.DataBase.DBTypes;
using Arcam.Data.DataTypes;

namespace Arcam.Market
{
    public interface IPlatform
    {
        List<Candle> TakeCandles(WorkingPair pair, int count, Timing timing, bool reverse = false, DateTimeOffset? where = null);
        string ClosePosition(WorkingPair pair);
        string PostOrders(bool buy, double quality, WorkingPair pair);
        Dictionary<string, PositionInfo> EncountPositions();
        WalletInfo TakeWallet(WorkingPair pair);
        string SetLeverage(WorkingPair pair, double leverage = 0);
        DateTime NextCheckDate();
        double TakeMidPrice(WorkingPair pair);
    }
}
