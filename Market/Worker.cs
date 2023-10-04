using Arcam.Data.DataTypes;
using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Indicators;
using Arcam.Main.Loggers;

namespace Arcam.Market
{
    public abstract class Worker
    {
        protected IPlatform _platform;
        protected Dictionary<string, PositionInfo> _currentPositions = new Dictionary<string, PositionInfo>();
        protected Dictionary<string, double> multiplier = new Dictionary<string, double>();
        protected Dictionary<string, PairSetting> _indicators = new Dictionary<string, PairSetting>();
        public CancellationToken ct;
        protected int index = 0;
        protected IIndicatorsSerializer sere;
        protected Logger logger = new Logger(typeof(Worker));

        public abstract void WorkerPreparer();
    }
}
