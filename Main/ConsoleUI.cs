using Arcam.Indicators.IndicatorsSerealizers;
using System.Runtime.InteropServices;

namespace Arcam.Main
{
    public class ConsoleUI
    {
        public static bool test = false;
        public static bool isLinux = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public delegate void Prepare(List<string> size);
        public delegate void Print(string vallet, Dictionary<string, string> data, IIndicatorsSerializer sere);
        public static Print PrintData = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ConsoleUILinux.PrintData : ConsoleUIWindows.PrintData;
        public static List<long> needStatus = new List<long>();
        public static void CheckStatus(long id)
        {
            needStatus.Add(id);
        }
    }
}
