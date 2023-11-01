using System.Text;

namespace Arcam.Main.Loggers
{
    public class Logger
    {
        public static bool isDebug = false;
        List<ILogger> loggers = new List<ILogger>();
        List<Type> types = new List<Type>() { typeof(ConsoleLogger), typeof(FileLogger), typeof(TelegramLogger) };

        public Logger(Type ShortFileName) : this(ShortFileName.Name) { }

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
            string res = data.ToString() ?? "";
            if (data is Exception)
            {
                res = GetErrorString(data as Exception ?? new Exception());
            }
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Log(res, "Error");
                }
                catch (Exception)
                {

                }
            }
        }
        public void Error(string symbol, object data)
        {
            string res = data.ToString() ?? "";
            if (data is Exception)
            {
                res = GetErrorString(data as Exception ?? new Exception());
            }
            foreach (var logger in loggers)
            {
                try
                {
                    logger.Log(symbol, res, "Error");
                }
                catch (Exception)
                {

                }
            }
        }
        public static string GetErrorString(Exception ex)
        {
            var str = new StringBuilder();
            str.Append($"{ex.GetType().ToString()}: {ex.Message}\n");
            var arr = (ex.StackTrace ?? "").Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var i = 0;
            while (i < arr.Length && !arr[i].Contains("Arcam")) i++;
            for (; i < arr.Length; i++)
            {
                str = str.Append(arr[i]).Append("\n");
            }
            return str.ToString();
        }
    }
}
