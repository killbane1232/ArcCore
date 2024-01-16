using NLog.Targets;

namespace Arcam.Main.Loggers
{
    class TelegramLogger : Target
    {
        public static TelegramBot bot = TelegramBot.getInstance();

        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                bot.SendTextMessage($"{logEvent.LoggerName} {logEvent.FormattedMessage}");
            }
            catch { }
        }
    }
}
