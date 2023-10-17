using Arcam.Data;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Arcam.Main
{
    class TelegramBot
    {
        private CancellationTokenSource cts;
        private TelegramBotClient client;
        private Dictionary<long, long> Users { get; set; } = new Dictionary<long, long>();
        private TelegramUserSerializer serializer;
        private string token;

        public TelegramBot()
        {
            var token = "";
            if (System.IO.File.Exists("./config/telegram.config"))
            {
                using (var reader = new StreamReader("./config/telegram.config"))
                {
                    token = reader.ReadLine();
                }
            }
            this.token = token;
            cts = new CancellationTokenSource();
            client = new TelegramBotClient(token);
            serializer = new TelegramUserSerializer();

            Users = serializer.ReadUsers() ?? new Dictionary<long, long>();

            client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null, cts.Token);
        }

        private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var result = Task.CompletedTask;
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message != null && update.Message.Type == MessageType.Text)
                        result = MessageHandler(update);
                    break;
                default:
                    result = Task.CompletedTask;
                    break;
            }

            return result;
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);

            Thread.Sleep(1000);
            client = new TelegramBotClient(token);
            client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null, cts.Token);
            return Task.CompletedTask;
        }

        private Task MessageHandler(Update update)
        {
            //return Task.CompletedTask;
            var msg = update.Message;

            if (msg == null || msg.From == null)
                return Task.CompletedTask;

            var result = Task.CompletedTask;

            switch (msg.Text)
            {
                case "/start":
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Привет! Подписка завершена!");
                    if (!Users.ContainsKey(msg.From.Id))
                    {
                        Users.Add(msg.From.Id, msg.Chat.Id);
                        serializer.SaveUsers(Users);
                    }
                    break;
                case "/stop":

                    Users.Remove(msg.From.Id);
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Регистрация успешно удалена.");
                    serializer.SaveUsers(Users);
                    break;
                case "/status":
                    ConsoleUI.needStatus = msg.From.Id;
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Статус запрошен.");
                    serializer.SaveUsers(Users);
                    break;
            }

            return result;
        }

        public void SendTextMessage(string message)
        {
            foreach (var item in Users)
            {
                client.SendTextMessageAsync(item.Key,
                        message);
            }
        }

        public void SendMdTableMessage(string message, long id)
        {
            foreach (var item in Users)
            {
                if (item.Key == id)
                {
                    client.SendTextMessageAsync(item.Key,
                        message, parseMode: ParseMode.Html);
                    return;
                }
            }
        }
    }
}
