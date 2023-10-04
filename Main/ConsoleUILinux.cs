﻿using Arcam.Indicators.IndicatorsSerealizers;
using System.Text;

namespace Arcam.Main
{
    public class ConsoleUILinux
    {
        static object locker = new object();
        static List<string> baseList = new List<string>();
        public static void PrepareMenu(int size)
        {
            ConsoleUI.size = size;
            if (!ConsoleUI.isLinux)
                return;
            for (int i = 0; i < size; i++)
                baseList.Add("");
        }
        static int printCnt = 0;
        public static void PrintData(string vallet, List<string> data, int index, IIndicatorsSerializer sere)
        {
            if (!ConsoleUI.isLinux)
                return;
            lock (locker)
            {
                printCnt++;
                var str = new StringBuilder("║Name");
                for (int i = 0; i < ConsoleUI.maxName - 3; i++)
                    str.Append(" ");
                str.Append("║Vallet    ");
                var indics = sere.GetIndicators();
                foreach (var each in indics)
                    str.Append($"║{each.Key}{new string(' ', 10 - each.Key.Length)}");
                str.Append("║Last update        ");
                baseList[0] = str.ToString();

                var name = Thread.CurrentThread.Name;
                str = new StringBuilder($"║{Thread.CurrentThread.Name}{new string(' ', ConsoleUI.maxName - name.Length + 1)}║{vallet}{new string(' ', 10 - vallet.ToString().Length)}");
                foreach (var each in data)
                    str.Append($"║{each}{new string(' ', 10 - each.Length)}");
                str.Append("║" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + "║");
                baseList[index + 1] = str.ToString();
                if (printCnt % ConsoleUI.size == 0)
                {
                    foreach (var each in baseList)
                        Console.WriteLine(each);
                }
            }
        }
    }
}
