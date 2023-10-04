using Arcam.Indicators.IndicatorsSerealizers;
using System.Text;

namespace Arcam.Main
{
    public delegate void Invoker(string data, int index);
    public class ConsoleUIWindows
    {
        static object locker = new object();
        public static void PrepareMenu(int size)
        {
            if (ConsoleUI.test)
                return;
            if (ConsoleUI.isLinux)
                return;

            ConsoleUI.size = size;
            var buffer = Console.BufferWidth - 2;
            var cnt = ConsoleUI.maxName + 1;
            var str = new StringBuilder("╔");
            for (int j = 1; j < buffer; j++)
            {
                if ((j == ConsoleUI.maxName + 2 || ((j - cnt) % 8 == 1 && j > ConsoleUI.maxName + 2)) && buffer - j - 19 > 0)
                    str.Append("╦");
                else
                    str.Append("═");
            }
            str.Append("╗");

            Console.WriteLine(str.ToString());
            Console.CursorLeft = 0;
            Console.CursorTop = 1;

            str = new StringBuilder("║Name");
            for (int i = 0; i < ConsoleUI.maxName - 3; i++)
                str.Append(" ");
            str.Append("║Vallet\t");

            Console.WriteLine(str.ToString());
            Console.CursorLeft = 0;
            Console.CursorTop = 2;

            for (int i = 0; i < size; i++)
            {
                str = new StringBuilder("╠");
                for (int j = 1; j < buffer; j++)
                    if ((j == ConsoleUI.maxName + 2 || ((j - cnt) % 8 == 1 && j > ConsoleUI.maxName + 2)) && buffer - j - 19 > 0)
                        str.Append("╬");
                    else
                        str.Append("═");
                str.Append("╣");
                Console.WriteLine(str.ToString());
                str = new StringBuilder("║");
                for (int j = 1; j < buffer; j++)
                    if ((j == ConsoleUI.maxName + 2 || ((j - cnt) % 8 == 1 && j > ConsoleUI.maxName + 2)) && buffer - j - 19 > 0)
                        str.Append("║");
                    else
                        str.Append(" ");
                str.Append("║");
                Console.WriteLine(str.ToString());
            }
            str = new StringBuilder("╚");
            for (int j = 1; j < buffer; j++)
                if (j == ConsoleUI.maxName + 2)
                    str.Append("╩");
                else
                if ((j - cnt) % 8 == 1 && j > ConsoleUI.maxName + 2 && buffer - j - 19 > 0)
                    str.Append("╩");
                else
                    str.Append("═");
            str.Append("╝");
            Console.WriteLine(str.ToString());
        }

        public static void PrintData(string vallet, List<string> data, int index, IIndicatorsSerializer sere)
        {
            if (ConsoleUI.test)
                return;
            if (ConsoleUI.isLinux)
                return;

            lock (locker)
            {
                var cnt = ConsoleUI.maxName;
                var dataStart = ConsoleUI.maxName + 11;
                var buffer = Console.BufferWidth - 2;
                Console.CursorLeft = dataStart;
                Console.CursorTop = 1;
                for (int j = dataStart; j < buffer; j++)
                    if (j == ConsoleUI.maxName + 2 || ((j - cnt) % 8 == 2 && j > ConsoleUI.maxName + 2))
                        Console.Write("║");
                    else
                        Console.Write(" ");
                Console.CursorLeft = dataStart - 1;
                //TODO: FIX THIS
                //var indics = sere.GetIndicators();
                //foreach (var each in indics)
                //    Console.Write($"║{each.Key} ");
                Console.Write("║");
                Console.CursorLeft = buffer - 19;
                Console.Write("Last update        ");
                Console.CursorLeft = buffer;
                Console.Write("║");
                Console.CursorTop = (index * 2) + 3;
                Console.CursorLeft = 1;
                for (int j = 0; j < buffer; j++)
                    if (j == ConsoleUI.maxName + 2 || ((j - cnt) % 8 == 1 && j > ConsoleUI.maxName + 2))
                        Console.Write("║");
                    else
                        Console.Write(" ");
                Console.CursorLeft = buffer - 19;
                Console.Write(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + "║");
                Console.CursorLeft = 0;
                Console.Write($"║{Thread.CurrentThread.Name}");
                Console.CursorLeft = ConsoleUI.maxName + 2;
                Console.Write($"║{vallet}");
                Console.CursorLeft = dataStart - 1;
            }
        }
    }
}
