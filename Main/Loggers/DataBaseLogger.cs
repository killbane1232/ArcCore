namespace Arcam.Main.Loggers
{
    internal class DataBaseLogger : ILogger
    {
        string baseName;
        private readonly string format = "HH:mm:ss.ffffzzz";
        public static bool isDebug = false;
        

        public DataBaseLogger(Type ShortFileName)
        {
            baseName = ShortFileName.Name;
        }

        public DataBaseLogger(string name)
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
