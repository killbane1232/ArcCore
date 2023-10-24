namespace Arcam.Main.Loggers
{
    class TelegramLogger : ILogger
    {
        public static TelegramBot bot = new TelegramBot();
        string baseName;
        private readonly string format = "HH:mm:ss.ffffzzz";
        public static bool isDebug = false;

        public TelegramLogger(Type ShortFileName)
        {
            baseName = ShortFileName.Name;
        }
        public TelegramLogger(string name)
        {
            baseName = name;
        }

        public void Debug(object data)
        {
            bot.SendTextMessage($"[{baseName} {DateTime.Now.ToString(format)}]-(Debug) \"{data}\"");
        }
        public void Debug(string symbol, object data)
        {
            bot.SendTextMessage($"[{baseName} {DateTime.Now.ToString(format)}]-(Debug {symbol}) \"{data}\"");
        }
        public void Log(object data, string type)
        {
            bot.SendTextMessage($"[{baseName} {DateTime.Now.ToString(format)}]-({type}) \"{data}\"");
        }
        public void Log(string symbol, object data, string type)
        {
            bot.SendTextMessage($"[{baseName} {DateTime.Now.ToString(format)}]-({type} {symbol}) \"{data}\"");
        }
    }
}
