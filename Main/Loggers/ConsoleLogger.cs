namespace Arcam.Main.Loggers
{
    internal class ConsoleLogger : ILogger
    {
        private readonly string baseName;
        private readonly string format = "HH:mm:ss.ffffzzz";
        public static bool isDebug = false;

        public ConsoleLogger(string name)
        {
            baseName = name;
        }
        public void Debug(object data)
        {
            Console.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-(Debug) \"{data}\"");
        }
        public void Debug(string symbol, object data)
        {
            Console.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-(Debug {symbol}) \"{data}\"");
        }
        public void Log(object data, string type)
        {
            Console.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-({type}) \"{data}\"");
        }
        public void Log(string symbol, object data, string type)
        {
            Console.WriteLine($"[{baseName} {DateTime.Now.ToString(format)}]-({type} {symbol}) \"{data}\"");
        }
    }
}
