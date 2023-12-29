using System.Text;

namespace Arcam.Main.Loggers
{
    public class LoggerConfigurator
    {
        public static void Configure()
        {
            LogManager.Setup().LoadConfiguration(builder => {
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: "file.txt");
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteTo(new TelegramLogger());
            });
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
