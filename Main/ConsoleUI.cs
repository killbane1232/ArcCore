using Arcam.Indicators.IndicatorsSerealizers;
using System.Runtime.InteropServices;

namespace Arcam.Main
{
    public class ConsoleUI
    {
        public static int maxName = 0;
        public static int size = 0;
        public static bool test = false;
        public static bool isLinux = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public delegate void Prepare(int size);
        public delegate void Print(string vallet, List<string> data, int index, IIndicatorsSerializer sere);
        public static Prepare PrepareMenu;
        public static Print PrintData;
    }
}
