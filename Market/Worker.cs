using Arcam.Data.DataTypes;
using Arcam.Main.Loggers;
using Arcam.Data.DataBase.DBTypes;
using NLog;

namespace Arcam.Market
{
    public abstract class Worker
    {
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        protected IPlatform _platform;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        protected Strategy _strategy = new();
        protected Account _account = new();
        public CancellationToken ct;
        protected Logger logger = LogManager.GetCurrentClassLogger();

        public abstract void Start();
    }
}
