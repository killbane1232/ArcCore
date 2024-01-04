using Arcam.Data;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;
using Arcam.Market;
using System.Security.Principal;

namespace Arcam.Main
{
    public class TelegramBot
    {
        private CancellationTokenSource cts;
        private TelegramBotClient client;
        private Dictionary<long, long> Users { get; set; } = new Dictionary<long, long>();
        private TelegramUserSerializer serializer;
        private static TelegramBot? bot;
        private static Dictionary<long, MenuItem> UserState = new Dictionary<long, MenuItem>();
        public static IPicker picker = null;
        public enum MenuItem
        {
            Start,
            Main,
            TestStrategy,
        }
        public static TelegramBot getInstance()
        {
            if (bot == null)
                bot = new TelegramBot();
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
                        "Добрый день! Введите логин пользователя для входа");
                    UserState[msg.Chat.Id] = MenuItem.Start;
                    break;
                case "/stop":
                    Users.Remove(Users.First(x => x.Value == msg.Chat.Id).Key);
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Регистрация успешно удалена.");
                    serializer.SaveUsers(Users);
                    break;
                case "/status":
                    StatusBuffer.CheckStatus(msg.From.Id);
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Статус запрошен.");
                    break;
                case "/test":
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Выберите стратегию для тестирования", replyMarkup: TestStratMenu(msg.Chat.Id));
                    UserState[msg.Chat.Id] = MenuItem.TestStrategy;
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
                            if(!UpdateUserTgId(msg.Text, msg.Chat.Id))
                            {
                                result = client.SendTextMessageAsync(msg.Chat.Id,
                                    "Неверный логин");
                                break;
                            }
                            result = client.SendTextMessageAsync(msg.Chat.Id,
                                "Добро пожаловать!");
                            UserState[msg.Chat.Id] = MenuItem.Main;
                            break;
                        case MenuItem.TestStrategy:
                            Strategy strat = null;
                            Account acc = null;
                            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
                            using (ApplicationContext db = new ApplicationContext())
                            {
                                strat = db.Strategy.Where(x => x.Name == msg.Text && x.AuthorId == userId).First();
                                if (strat == null)
                                {
                                    result = client.SendTextMessageAsync(msg.Chat.Id,
                                        "Стратегии с таким названием не существует!");
                                    break;
                                }
                                acc = db.Account.Where(x => x.StrategyId == strat.Id && x.UserId == userId).First();
                                db.Entry(acc).Reference(x => x.Strategy).Load();
                                db.Entry(acc).Reference(x => x.Platform).Load();
                                //Type platformType = Type.GetType(eachAcc.Platform.ClassName);
                                db.Entry(strat).Reference(x => x.Timing).Load();
                                db.Entry(strat).Reference(x => x.Pair).Load();
                                db.Entry(strat).Collection(x => x.StrategyIndicators).Load();

                                foreach (var indicator in strat.StrategyIndicators)
                                {
                                    db.Entry(indicator).Reference(x => x.Indicator).Load();
                                    var fields = db.InputField.Where(x => x.StrategyIndicatorId == indicator.Id).ToList();
                                    foreach (var field in fields)
                                    {
                                        db.Entry(field).Reference(x => x.IndicatorField).Load();
                                        indicator.InputFields.Add(field.IndicatorField.CodeName, field);
                                    }
                                }
                            }
                            if (acc == null)
                            {
                                result = client.SendTextMessageAsync(msg.Chat.Id,
                                    "Аккаунт для данной стратегии не найден!");
                                break;
                            }

                            if (picker != null)
                            {
                                picker.PickIndicators(strat, acc);
                                result = client.SendTextMessageAsync(msg.Chat.Id,
                                    "Стратегия успешно протестирована!");
                            }
                            break;
                        default:
                            client.SendStickerAsync(msg.Chat.Id, InputFile.FromFileId("CAACAgIAAxkBAAIBmWFf8Ia0tHtyLUI9Pg2cfe2Pz87tAAIuAwACtXHaBqoozbmcyVK2IQQ"));
                            break;
                    }
                    break;

            }

            return result;
        }

        public IReplyMarkup TestStratMenu(long chatId)
        {
            var keyboardList = new List<KeyboardButton>();
            var userId = Users.First(x => x.Value == chatId).Key;
            using (ApplicationContext db = new ApplicationContext())
            {
                var accs = db.Account.Where(x => x.UserId == userId).ToList();
                foreach (var acc in accs)
                {
                    db.Entry(acc).Reference(x => x.Strategy).Load();
                    keyboardList.Add(new KeyboardButton(acc.Strategy.Name));
                    Console.WriteLine(acc.Strategy.Name);
                }
            }

            return new ReplyKeyboardMarkup(keyboardList);
        }

        public void SendTextMessage(string message)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var admins = db.User.Where(x => x.Access == 0).ToList();
                foreach (var item in admins)
                {
                    client.SendTextMessageAsync(Users[item.Id],
                            message);
                }
            }
        }

        public void SendMdTableMessage(string message, long id)
        {
            foreach (var item in Users)
            {
                if (item.Value == id)
                {
                    client.SendTextMessageAsync(item.Value,
                        message, parseMode: ParseMode.Html);
                    return;
                }
            }
        }

        public bool UpdateUserTgId(string login, long chatId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.User.Where(x => x.Login == login);
                if (users.Count() == 0)
                    return false;
                var user = users.First();
                if (user == null) return false;
                user.TelegramId = chatId;
                db.User.Update(user);
                db.SaveChanges();
                Users[user.Id] = chatId;
                serializer.SaveUsers(Users);
                return true;
            }
        }
    }
}
