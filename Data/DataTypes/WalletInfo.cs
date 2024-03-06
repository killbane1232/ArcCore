namespace Arcam.Data.DataTypes
{
    public class WalletInfo
    {
        //public long amount;
        //public long? pendingDebit;
        //public long? account;
        public long marginBalance = 0;
        public long AvaivableToTrade
        {
            get => marginBalance / 10000;
            //get => (amount - (pendingDebit.HasValue ? pendingDebit.Value : 0)) / 10000;
        }
        public long AvaivableToTradeWhenClosed
        {
            get => marginBalance / 10000;
            //get => (amount - (pendingDebit.HasValue ? pendingDebit.Value : 0)) / 10000;
        }
        public double MyDelimeter;
        public string Postfix = "";
        public string AvaivableToTradeWhenClosedString
        {
            get => (AvaivableToTradeWhenClosed / MyDelimeter).ToString() + Postfix;
            //get => (amount - (pendingDebit.HasValue ? pendingDebit.Value : 0)) / 10000;
        }
    }
}
