using Arcam.Main.Loggers;
using System.Text;

namespace Arcam.Main
{
    public class StatusBuffer
    {
        public static bool IsTest = false;
        private static List<long> NeedStatus = new();
        private static object locker = new object();
        private static List<string> keysList = new List<string>();
        private static Dictionary<string, Dictionary<string, string>> baseList = new Dictionary<string, Dictionary<string, string>>();
        private static int printCnt = 0;
        private static int maxNameLength = 0;
        private static int maxValletLength = 6;

        public static void AddDataToBuffer(string vallet, Dictionary<string, string> data)
        {
            if (IsTest)
                return;
            string date = "Last update";
            string valletStr = "Vallet";

            lock (locker)
            {
                ClientThreadPool.SetLastResponse();
                printCnt++;
                Dictionary<string, string>? currentThreadDict = null;
                var curName = Thread.CurrentThread.Name ?? "";
                if (!baseList.ContainsKey(curName))
                {
                    currentThreadDict = new Dictionary<string, string>();
                    baseList[curName] = currentThreadDict;
                    if (maxNameLength < curName.Length)
                        maxNameLength = curName.Length;
                }
                else
                    currentThreadDict = baseList[curName];
                currentThreadDict[valletStr] = vallet;

                if (maxValletLength < vallet.Length)
                    maxValletLength = vallet.Length;

                foreach (var each in data)
                {
                    if (!keysList.Contains(each.Key))
                    {
                        keysList.Add(each.Key);
                        keysList.Sort();
                    }
                    currentThreadDict[each.Key] = data[each.Key];
                }
                currentThreadDict[date] = DateTime.Now.ToString("dd.MM HH:mm:ss");

                if ((printCnt % baseList.Count) == 0 && NeedStatus.Count > 0)
                {
                    printCnt = 0;
                    var str = new StringBuilder();
                    str.Append(new string(' ', maxNameLength)).Append("║").Append("Vallet");
                    if (maxValletLength > 6)
                    {
                        str.Append(new string(' ', maxValletLength - 6));
                    }
                    for (var i = 0; i < keysList.Count; i++)
                        str.Append($"║{keysList[i]}");
                    str.Append($"║{date}\n");
                    foreach (var each in baseList.OrderBy(x => x.Key))
                    {
                        str.Append(each.Key);
                        if (each.Key.Length < maxNameLength)
                        {
                            str.Append(new string(' ', maxNameLength - each.Key.Length));
                        }
                        str.Append("║").Append(each.Value[valletStr]);
                        if (each.Value[valletStr].Length < maxValletLength)
                        {
                            str.Append(new string(' ', maxValletLength - each.Value[valletStr].Length));
                        }
                        for (var i = 0; i < keysList.Count; i++)
                            str.Append("║" + each.Value[keysList[i]] + new string(' ', keysList[i].Length - each.Value[keysList[i]].Length));
                        str.Append($"║{each.Value[date]}\n");
                    }
                    str.Insert(0, "<pre>\n");
                    str.Append("</pre>\n");
                    var userIndex = 0;
                    while (userIndex < NeedStatus.Count)
                    {
                        try
                        {
                            TelegramLogger.bot.SendMdTableMessage(str.ToString(), NeedStatus[userIndex]);
                            NeedStatus.RemoveAt(userIndex);
                        }
                        catch
                        {
                            userIndex++;
                        }
                    }
                }
            }
        }
        public static void CheckStatus(long id)
        {
            NeedStatus.Add(id);
        }
    }
}
