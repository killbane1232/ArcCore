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
        protected Dictionary<string, PositionInfo> _currentPositions = new Dictionary<string, PositionInfo>();
        protected Dictionary<string, double> multiplier = new Dictionary<string, double>();
        protected Strategy _indicators = new Strategy();
        public CancellationToken ct;
        protected int index = 0;
        protected Logger logger = LogManager.GetCurrentClassLogger();

        public abstract void Start();
    }
}
