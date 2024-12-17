using Arcam.Data;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Arcam.Data.DataBase;

namespace Arcam.Main
{
    public class TelegramBot
    {
        private readonly CancellationTokenSource cts;
        private readonly TelegramBotClient client;
        private Dictionary<long, long> Users { get; set; } = [];
        private static TelegramBot? bot;
        private static readonly Dictionary<long, MenuItem> UserState = [];
        public static IPicker? picker = null;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        public enum MenuItem
        {
            Start,
            Main,
            TestStrategy,
            GetStrategy,
            SetupStrategy,
            AddIndicator,
            SetupField,
            SetupValue
        }
        public static TelegramBot getInstance()
        {
            bot ??= new TelegramBot();
            return bot;
        }

        private TelegramBot()
        {
            var token = "";
            if (System.IO.File.Exists($"{Constants.ConfigDirectory}/telegram.config"))
            {
                using (var reader = new StreamReader($"{Constants.ConfigDirectory}/telegram.config"))
                {
                    token = reader.ReadLine() ?? "";
                }
            }
            cts = new CancellationTokenSource();
            client = new TelegramBotClient(token);
            Users = new Dictionary<long, long>();
            using (var db = new ProdContext())
            {
                foreach (var item in db.User.Where(x => x.TelegramId != null))
                {
                    Users[item.Id] = item.TelegramId ?? 0;
                }
            }

            client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null, cts.Token);
        }

        private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var result = Task.CompletedTask;
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message != null && update.Message.Type == MessageType.Text)
                        try
                        {
                            result = MessageHandler(update);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            result = client.SendTextMessageAsync(update.Message.Chat.Id,
                                "Произошла какая-то ошибка!", replyMarkup: new ReplyKeyboardRemove());
                        }
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
            logger.Error(errorMessage);

            return Task.CompletedTask;
        }

        private Task MessageHandler(Update update)
        {
            var msg = update.Message;

            if (msg == null || msg.From == null || msg.Text == null)
                return Task.CompletedTask;

            var result = Task.CompletedTask;

            switch (msg.Text)
            {
                case "/start":
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Добрый день! Введите логин пользователя для входа:");
                    UserState[msg.Chat.Id] = MenuItem.Start;
                    break;
                case "/stop":
                    using (var db = new ProdContext())
                    {
                        foreach (var item in db.User.Where(x => x.TelegramId == msg.Chat.Id))
                        {
                            item.TelegramId = null;
                            db.User.Update(item);
                        }
                    }
                    Users.Remove(Users.First(x => x.Value == msg.Chat.Id).Key);
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Регистрация успешно удалена.");
                    break;
                case "/status":
                    StatusBuffer.CheckStatus(msg.From.Id);
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Статус запрошен.");
                    break;
                default:
                    if (!UserState.ContainsKey(msg.Chat.Id))
                    {
                        client.SendStickerAsync(msg.Chat.Id, InputFile.FromFileId("CAACAgIAAxkBAAIBmWFf8Ia0tHtyLUI9Pg2cfe2Pz87tAAIuAwACtXHaBqoozbmcyVK2IQQ"));
                        break;
                    }
                    switch (UserState[msg.Chat.Id])
                    {
                        case MenuItem.Start:
                            if (!UpdateUserTgId(msg.Text, msg.Chat.Id))
                            {
                                result = client.SendTextMessageAsync(msg.Chat.Id,
                                    "Неверный логин");
                                break;
                            }
                            result = client.SendTextMessageAsync(msg.Chat.Id,
                                "Добро пожаловать!\n");
                            UserState[msg.Chat.Id] = MenuItem.Main;
                            break;
                        default:
                            client.SendStickerAsync(msg.Chat.Id, InputFile.FromFileId("CAACAgIAAxkBAAIBmWFf8Ia0tHtyLUI9Pg2cfe2Pz87tAAIuAwACtXHaBqoozbmcyVK2IQQ"));
                            break;
                    }
                    break;

            }

            return result;
        }

        public void SendTextMessage(string message)
        {
            if (message.Contains("Arcam.Main.TelegramBot") && message.Contains("Telegram.Bot.Exceptions.RequestException: Request timed out"))
                return;
            using ApplicationContext db = new ProdContext();
            var admins = db.User.Where(x => x.TelegramId != null).ToList();
            foreach (var user in admins)
            {
                db.Entry(user).Reference(x => x.Access).Load();
                db.Entry(user.Access).Collection(x => x.MatrixParameters).Load();
                foreach (var param in user.Access.MatrixParameters)
                {
                    db.Entry(param).Reference(x => x.AccessType).Load();
                }
            }
            foreach (var item in admins.Where(x => x.Access.MatrixParameters.Any(y => y.AccessType.Name == "admin")))
            {
                client.SendTextMessageAsync(item.TelegramId!,
                    message);
            }
        }

        public void SendMdTableMessage(string message, long id)
        {
            if (Users.ContainsValue(id))
            {
                client.SendTextMessageAsync(id,
                    message, parseMode: ParseMode.Html);
                return;
            }
        }

        public bool UpdateUserTgId(string login, long chatId)
        {
            using ApplicationContext db = new ProdContext();
            var users = db.User.Where(x => x.Login == login);
            if (!users.Any())
                return false;
            var user = users.First();
            if (user == null) return false;
            user.TelegramId = chatId;
            db.User.Update(user);
            db.SaveChanges();
            Users[user.Id] = chatId;
            return true;
        }
    }
}
