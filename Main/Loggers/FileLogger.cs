using Arcam.Data;

namespace Arcam.Main.Loggers
{
    internal class FileLogger : ILogger
    {
        string baseName;
        private readonly string format = "HH:mm:ss.ffffzzz";
        public static bool isDebug = false;

        public FileLogger(string name)
        {
            baseName = name;
        }
        public void Debug(object data)
        {
            try
            {
                var writer = new StreamWriter($"{Constants.LogDirectory}/{Thread.CurrentThread.Name} {DateTime.Now.ToLocalTime().ToShortDateString().Replace('/', '.')}-Debug.txt", true);
                writer.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-(Debug) \"{data}\"");
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Debug(string symbol, object data)
        {
            try
            {
                var writer = new StreamWriter($"{Constants.LogDirectory}/{Thread.CurrentThread.Name} {DateTime.Now.ToLocalTime().ToShortDateString().Replace('/', '.')}-Debug.txt", true);
                writer.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-(Debug {symbol}) \"{data}\"");
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Info(object data)
        {
            Log(data, "Info");
        }
        public void Info(string symbol, object data)
        {
            Log(symbol, data, "Info");
        }
        public void Error(object data)
        {
            Log(data, "Error");
        }
        public void Error(string symbol, object data)
        {
            Log(symbol, data, "Error");
        }
        public void Log(object data, string type)
        {
            try
            {
                var writer = new StreamWriter($"{Constants.LogDirectory}/{Thread.CurrentThread.Name} {DateTime.Now.ToLocalTime().ToShortDateString().Replace('/', '.')}.txt", true);
                writer.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-({type}) \"{data}\"");
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Log(string symbol, object data, string type)
        {
            try
            {
                var writer = new StreamWriter($"{Constants.LogDirectory}/{Thread.CurrentThread.Name} {DateTime.Now.ToLocalTime().ToShortDateString().Replace('/', '.')}.txt", true);
                writer.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-({type} {symbol}) \"{data}\"");
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
