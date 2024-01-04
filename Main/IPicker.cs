using Arcam.Data.DataBase.DBTypes;

namespace Arcam.Main
{
    public interface IPicker
    {
        public TestStrategy PickIndicators(Strategy strategy, Account account);
    }
}