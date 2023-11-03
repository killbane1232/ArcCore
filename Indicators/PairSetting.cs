namespace Arcam.Indicators
{
    public class PairSetting
    {
        public bool canLong = false;
        public bool canShort = false;
        public int leverage = 0;
        public int timeSpan = 0;
        public string pair = "";
        public List<IndicatorParams> indicators = new List<IndicatorParams>();
        public IndicatorParams this[string idx]
        {
            get
            {
                for (int i = 0; i < indicators.Count; i++)
                {
                    if (indicators[i].name.StartsWith(idx))
                        return indicators[i];
                }
                return IndicatorParams.empty;
            }
        }
        public void Update(List<PickParam> Values)
        {
            for (int j = 0; j < indicators.Count; j++)
            {
                if (indicators[j].name.StartsWith(Values[0].className))
                    indicators[j].parameters[Values[0].paramName]++;
            }
            int i = 0;
            for (; i < Values.Count - 1; i++)
            {
                if (this[Values[i].className].name == "")
                    return;
                if (this[Values[i].className].parameters[Values[i].paramName] > Values[i].MaxValue)
                {
                    for (int j = 0; j < indicators.Count; j++)
                    {
                        if (indicators[j].name.StartsWith(Values[i].className))
                            indicators[j].parameters[Values[i].paramName] = Values[i].MinValue;
                    }
                    for (int j = 0; j < indicators.Count; j++)
                    {
                        if (indicators[j].name.StartsWith(Values[i + 1].className))
                            indicators[j].parameters[Values[i + 1].paramName]++;
                    }
                }
                else
                    break;
            }
        }
    }
}
