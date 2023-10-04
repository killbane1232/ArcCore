namespace Arcam.Main.Loggers
{
    internal interface ILogger
    {
        public void Debug(object data);
        public void Debug(string symbol, object data);
        public void Log(object data, string type);
        public void Log(string symbol, object data, string type);
    }
}
