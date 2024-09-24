using NLog.Targets;

namespace Arcam.Main.Loggers
{
    class TelegramLogger : Target
    {
        public static TelegramBot bot = TelegramBot.getInstance();
        Logger logger = LogManager.GetCurrentClassLogger();
        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                bot.SendTextMessage($"{logEvent.LoggerName} {logEvent.FormattedMessage}");
            }
            catch (Exception e) {
                logger.Error(e);
            }
        }
    }
}
