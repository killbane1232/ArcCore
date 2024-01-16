using Arcam.Data.DataBase.DBTypes;

namespace Arcam.Main
{
    public interface IPicker
    {
        public string PickIndicators(Strategy strategy, Account account);
    }
}