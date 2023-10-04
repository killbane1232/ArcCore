namespace Arcam.Indicators
{
    public class IndicatorParams
    {
        public string name = "";
        public string classname = "";
        public Dictionary<string, int> parameters = new Dictionary<string, int>();
        public bool? isExit = false;

        public static IndicatorParams empty = new IndicatorParams();
    }
}
