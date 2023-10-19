using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Main.Loggers;
using System.Text;

namespace Arcam.Main
{
    public class ConsoleUILinux
    {
        static object locker = new object();
        public static List<List<string>> baseList = new List<List<string>>();
        public static void PrepareMenu(int size)
        {
            ConsoleUI.size = size;
            if (!ConsoleUI.isLinux)
                return;
            for (int i = -1; i < size; i++)
                baseList.Add(new List<string>());
            baseList[0] = new List<string>();
            baseList[0].Add("Name");
            baseList[0].Add("Vallet");
        }
        static int printCnt = 0;
        const string date = "║Last update   ";
        public static void PrintData(string vallet, List<string> data, int index, IIndicatorsSerializer sere)
        {
            if (!ConsoleUI.isLinux)
                return;
            lock (locker)
            {
                printCnt++;
                baseList[0] = new List<string>
                {
                    "Name",
                    "Vallet"
                };
                baseList[index + 1] = new List<string>
                {
                    Thread.CurrentThread.Name,
                    vallet
                };
                var indics = sere.GetIndicators();
                foreach (var each in indics)
                {
                    var first = baseList[0].FindIndex(x => x == each.Key);
                    if (first == -1)
                        baseList[0].Add(each.Key);
                }
                foreach (var each in data)
                    baseList[index + 1].Add(each);
                baseList[index + 1].Add(DateTime.Now.ToString("dd.MM HH:mm:ss"));

                if (printCnt % ConsoleUI.size == 0)
                {
                    var maxList = new List<int>();
                    for(int i = 0; i < baseList[0].Count; i++)
                    {
                        var max = 0;
                        for(int j = 0; j < baseList.Count; j++)
                            if (baseList[j][i].Length > max)
                                max = baseList[j][i].Length;
                        maxList.Add(max);
                    }
                    foreach (var each in baseList) {
                        var str = new StringBuilder();
                        for (var i = 0; i < each.Count; i++)
                            str.Append("║" + each[i] + (maxList.Count > i && maxList[i] > each[i].Length ? new string(' ', maxList[i] - each[i].Length) : ""));
                        if (each.Count == baseList[0].Count)
                        {
                            str.Append(date);
                        }
                        Console.WriteLine(str.ToString());
                    }
                    if (ConsoleUI.needStatus > -1)
                    {
                        try
                        {
                            var statuses = new StringBuilder();
                            statuses.Append("<pre>\n");
                            foreach (var each in baseList)
                            {
                                var str = new StringBuilder();
                                for (var i = 0; i < each.Count; i++)
                                    str.Append("║" + each[i] + (maxList.Count > i && maxList[i] > each[i].Length ? new string(' ', maxList[i] - each[i].Length) : ""));
                                if (each.Count == baseList[0].Count)
                                {
                                    str.Append(date);
                                }
                                statuses.Append(str).Append('\n');
                            }
                            statuses.Append("\n</pre>");
                            TelegramLogger.bot.SendMdTableMessage(statuses.ToString(), ConsoleUI.needStatus);
                        }
                        catch
                        {

                        }
                        ConsoleUI.needStatus = -1;
                    }
                }
            }
        }
    }
}
