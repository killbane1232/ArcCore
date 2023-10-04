namespace Arcam.Indicators.IndicatorsSerealizers
{
    public interface IIndicatorsSerializer
    {
        public Dictionary<string, PairSetting> GetIndicators();
        public void LoadIndicator();
        public void SaveIndicator();
    }
}
