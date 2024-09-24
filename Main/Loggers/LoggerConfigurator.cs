using System.Text;

namespace Arcam.Main.Loggers
{
    public class LoggerConfigurator
    {
        public enum LogType {
            main,
            site,
            test
        }
        public static void Configure(LogType type = LogType.main)
        {
            switch (type) {
                case LogType.test:
                    LogManager.Setup().LoadConfiguration(builder => {
                        builder.ForLogger().FilterMinLevel(LogLevel.Debug).FilterMaxLevel(LogLevel.Debug).WriteToConsole();
                        builder.ForLogger().FilterMinLevel(LogLevel.Debug).FilterMaxLevel(LogLevel.Debug).WriteToFile(fileName: "logs/Generations.txt");
                    });
                    return;
                case LogType.site:
                    LogManager.Setup().LoadConfiguration(builder => {
                        builder.ForLogger().FilterMinLevel(LogLevel.Warn).WriteToConsole();
                        builder.ForLogger().FilterMinLevel(LogLevel.Warn).WriteToFile(fileName: "logs/Generations.txt");
                    });
                    return;
                case LogType.main:
                    LogManager.Setup().LoadConfiguration(builder => {
                        builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToConsole();
                        builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: "logs/${shortdate}/${logger}.txt");
                        builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteTo(new TelegramLogger());
                    
                    });
                    return;
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
