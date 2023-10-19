using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Main.Loggers;
using System.Text;

namespace Arcam.Main
{
    public delegate void Invoker(string data, int index);
    public class ConsoleUIWindows
    {
        static object locker = new object();
        public static List<string> keysList = new List<string>();
        public static List<string> namesList = new List<string>();
        public static Dictionary<string, Dictionary<string, string>> baseList = new Dictionary<string, Dictionary<string, string>>();
        static int printCnt = 0;
        const string date = "Last update";
        const string valletStr = "Vallet";
        public static void PrepareMenu(List<string> names)
        {
            ConsoleUI.size = names.Count;
            namesList = names;
        }

        public static void PrintData(string vallet, Dictionary<string, string> data, IIndicatorsSerializer sere)
        {
            if (ConsoleUI.test)
                return;

            lock (locker)
            {
                printCnt++;
                Dictionary<string, string>? currentThreadDict = null;
                if (!baseList.ContainsKey(Thread.CurrentThread.Name ?? ""))
                {
                    currentThreadDict = new Dictionary<string, string>();
                    currentThreadDict.Add(valletStr, vallet);
                    baseList[Thread.CurrentThread.Name ?? ""] = currentThreadDict;
                }
                else
                    currentThreadDict = baseList[Thread.CurrentThread.Name ?? ""];
                currentThreadDict[valletStr] = vallet;
                var indics = sere.GetIndicators();
                foreach (var each in indics)
                {
                    if (!keysList.Contains(each.Key))
                    {
                        keysList.Add(each.Key);
                        keysList.Sort();
                    }
                    currentThreadDict[each.Key] = data[each.Key];
                }
                currentThreadDict[date] = DateTime.Now.ToString("dd.MM HH:mm:ss");

                if (printCnt % ConsoleUI.size == 0)
                {
                    var maxName = 0;
                    var maxVallet = 6;
                    var str = new StringBuilder();
                    for (var i = 0; i < ConsoleUI.size; i++)
                    {
                        if (maxName < namesList[i].Length)
                            maxName = namesList[i].Length;
                        if (baseList.ContainsKey(namesList[i]) && baseList[namesList[i]].ContainsKey(valletStr) && maxVallet < baseList[namesList[i]][valletStr].Length)
                            maxVallet = baseList[namesList[i]][valletStr].Length;
                    }
                    str.Append(new string(' ', maxName)).Append("║").Append("Vallet");
                    if (maxVallet > 6)
                    {
                        str.Append(new string(' ', maxVallet - 6));
                    }
                    for (var i = 0; i < keysList.Count; i++)
                        str.Append($"║{keysList[i]}");
                    str.Append($"║{date}\n");
                    foreach (var each in baseList)
                    {
                        str.Append(each.Key);
                        if (each.Key.Length < maxName)
                        {
                            str.Append(new string(' ', maxName - each.Key.Length));
                        }
                        str.Append("║").Append(each.Value[valletStr]);
                        if (each.Value[valletStr].Length < maxVallet)
                        {
                            str.Append(new string(' ', maxVallet - each.Value[valletStr].Length));
                        }
                        for (var i = 0; i < keysList.Count; i++)
                            str.Append("║" + each.Value[keysList[i]] + new string(' ', keysList[i].Length - each.Value[keysList[i]].Length));
                        str.Append($"║{each.Value[date]}\n");
                    }
                    Console.WriteLine(str.ToString());
                    if (ConsoleUI.needStatus > -1)
                    {
                        try
                        {
                            str.Insert(0, "<pre>\n");
                            str.Append("</pre>\n");
                            TelegramLogger.bot.SendMdTableMessage(str.ToString(), ConsoleUI.needStatus);
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
