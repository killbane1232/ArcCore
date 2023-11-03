using Arcam.Data.DataTypes;
using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Indicators;
using Arcam.Main.Loggers;

namespace Arcam.Market
{
    public abstract class Worker
    {
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        protected IPlatform _platform;
        protected IIndicatorsSerializer sere;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        protected Dictionary<string, PositionInfo> _currentPositions = new Dictionary<string, PositionInfo>();
        protected Dictionary<string, double> multiplier = new Dictionary<string, double>();
        protected PairSetting _indicators = new PairSetting();
        public CancellationToken ct;
        protected int index = 0;
        protected Logger logger = new Logger(typeof(Worker));

        public abstract void Start();
    }
}
