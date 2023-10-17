namespace Arcam.Main.Loggers
{
    public class Logger
    {
        public static bool isDebug = false;
        List<ILogger> loggers = new List<ILogger>();
        List<Type> types = new List<Type>() { typeof(ConsoleLogger), typeof(FileLogger), typeof(TelegramLogger) };

        public Logger(Type ShortFileName)
        {
            foreach (var type in types)
            {
                try
                {
                    loggers.Add((ILogger)type.GetConstructors().First(x => x.GetParameters().Any(y => y.ParameterType == typeof(Type))).Invoke(new object[] { ShortFileName }));
                }
                catch { }
            }
        }

        public Logger(string name)
        {
            foreach (var type in types)
            {
                try
                {
                    loggers.Add((ILogger)type.GetConstructors().First(x => x.GetParameters().Any(y => y.ParameterType == typeof(string))).Invoke(new object[] { name }));
                }
                catch { }
            }
        }
        public void Debug(object data)
        {
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Debug(data);
                }
                catch (Exception)
                {

                }
            }
        }
        public void Debug(string symbol, object data)
        {
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Debug(symbol, data);
                }
                catch (Exception)
                {

                }
            }
        }
        public void Info(object data)
        {
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Log(data, "Info");
                }
                catch (Exception)
                {

                }
            }
        }
        public void Info(string symbol, object data)
        {
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Log(symbol, data, "Info");
                }
                catch (Exception)
                {

                }
            }
        }
        public void Error(object data)
        {
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Log(data, "Error");
                }
                catch (Exception)
                {

                }
            }
        }
        public void Error(string symbol, object data)
        {
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Log(symbol, data, "Error");
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
