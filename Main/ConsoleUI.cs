using Arcam.Indicators.IndicatorsSerealizers;
using System.Runtime.InteropServices;

namespace Arcam.Main
{
    public class ConsoleUI
    {
        public static int maxName = 0;
        public static int size = 0;
        public static bool test = false;
        public static bool isLinux = true;//!RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public delegate void Prepare(int size);
        public delegate void Print(string vallet, List<string> data, int index, IIndicatorsSerializer sere);
        public static Prepare PrepareMenu = (size)=> { };
        public static Print PrintData = (vallet, data, index, sere) => { };
        public static long needStatus = -1;
        public static void CheckStatus(long id)
        {
            needStatus = id;
        }
    }
}
