using Arcam.Indicators.IndicatorsSerealizers;
using System.Runtime.InteropServices;

namespace Arcam.Main
{
    public class ConsoleUI
    {
        public static int size = 0;
        public static bool test = false;
        public static bool isLinux = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public delegate void Prepare(List<string> size);
        public delegate void Print(string vallet, Dictionary<string, string> data, IIndicatorsSerializer sere);
        public static Prepare PrepareMenu = (names) => { };
        public static Print PrintData = (vallet, data, sere) => { };
        public static List<long> needStatus = new List<long>();
        public static void CheckStatus(long id)
        {
            needStatus.Add(id);
        }
    }
}
