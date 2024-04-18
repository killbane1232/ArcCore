﻿using System.Text;

namespace Arcam.Main.Loggers
{
    public class LoggerConfigurator
    {
        public static void Configure(bool flag = false)
        {
            if (flag)
            {
                LogManager.Setup().LoadConfiguration(builder => {
                    builder.ForLogger().FilterMinLevel(LogLevel.Warn).WriteToConsole();
                    builder.ForLogger().FilterMinLevel(LogLevel.Warn).WriteToFile(fileName: "logs/Generations.txt");
                });
                return;
            }
            LogManager.Setup().LoadConfiguration(builder => {
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToConsole();
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: "logs/${shortdate}/${logger}.txt");
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteTo(new TelegramLogger());
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
