using Arcam.Data;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;
using System.IO;

namespace Arcam.Main
{
    public class TelegramBot
    {
        private CancellationTokenSource cts;
        private TelegramBotClient client;
        private Dictionary<long, long> Users { get; set; } = new Dictionary<long, long>();
        private Dictionary<long, Strategy> StrategyUserWorkingOn { get; set; } = new Dictionary<long, Strategy>();
        private Dictionary<long, StrategyIndicator> IndicatorUserWorkingOn { get; set; } = new Dictionary<long, StrategyIndicator>();
        private Dictionary<long, InputField> FieldUserWorkingOn { get; set; } = new Dictionary<long, InputField>();
        private TelegramUserSerializer serializer;
        private static TelegramBot? bot;
        private static Dictionary<long, MenuItem> UserState = new Dictionary<long, MenuItem>();
        public static IPicker picker = null;
        private Logger logger = LogManager.GetCurrentClassLogger();
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
            logger.Debug(errorMessage);

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
                        "Добрый день! Введите логин пользователя для входа:");
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
                        "Выберите стратегию для тестирования:", replyMarkup: AllStratsMenu(msg.Chat.Id));
                    UserState[msg.Chat.Id] = MenuItem.TestStrategy;
                    break;
                case "/strategy_settings":
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Выберите стратегию для настройки или введите название новой:", replyMarkup: AllStratsMenu(msg.Chat.Id));
                    UserState[msg.Chat.Id] = MenuItem.GetStrategy;
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
                            result = TestStrategy(msg);
                            break;
                        case MenuItem.GetStrategy:
                            result = GetStrategy(msg);
                            break;
                        case MenuItem.SetupStrategy:
                            result = SetupStrategy(msg);
                            break;
                        case MenuItem.AddIndicator: 
                            result = AddIndicator(msg); 
                            break;
                        case MenuItem.SetupField:
                            result = SetupField(msg);
                            break;
                        case MenuItem.SetupValue:
                            result = SetupValue(msg);
                            break;
                        default:
                            client.SendStickerAsync(msg.Chat.Id, InputFile.FromFileId("CAACAgIAAxkBAAIBmWFf8Ia0tHtyLUI9Pg2cfe2Pz87tAAIuAwACtXHaBqoozbmcyVK2IQQ"));
                            break;
                    }
                    break;

            }

            return result;
        }
        public Task GetStrategy(Message msg)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            if (msg.Text == "Back")
            {
                UserState[msg.Chat.Id] = MenuItem.Main;
                return client.SendTextMessageAsync(msg.Chat.Id,
                "Ok", replyMarkup: new ReplyKeyboardRemove());
            }
            Strategy strat = null;
            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
            using (ApplicationContext db = new ApplicationContext())
            {
                var strats = db.Strategy.Where(x => x.Name == msg.Text && x.AuthorId == userId);
                if (strats.Count() == 0)
                {
                    strat = new Strategy
                    {
                        Name = msg.Text,
                        AuthorId = userId,
                        IsPublic = false,
                        IsShort = false,
                        IsLong = false,
                        Leverage = 1,
                        ModUserId = userId,
                        PairId = null,
                        TimingId = null,
                        StrategyIndicators = new List<StrategyIndicator>()
                    };
                    db.Strategy.Add(strat);
                    db.SaveChanges();
                }
                else
                {
                    var cur = 1;
                    var curName = "";
                    strat = strats.First();
                    db.Entry(strat).Collection(x => x.StrategyIndicators).Load();
                    foreach (var indic in strat.StrategyIndicators.OrderBy(x => x.Id))
                    {
                        db.Entry(indic).Reference(x => x.Indicator).Load();
                        if (curName == indic.Indicator.Name)
                        {
                            cur++;
                        }
                        else
                        {
                            curName = indic.Indicator.Name;
                            cur = 1;
                        }
                        keyboardList.Add(new() { new KeyboardButton(indic.Indicator.Name + ": " + cur) });
                    }
                }
            }
            keyboardList.Add(new() { new KeyboardButton("Add new") });
            StrategyUserWorkingOn[userId] = strat;
            UserState[msg.Chat.Id] = MenuItem.SetupStrategy;
            return client.SendTextMessageAsync(msg.Chat.Id,
                "Выберите индикатор для настройки или создайте новый:", replyMarkup: new ReplyKeyboardMarkup(keyboardList));
        }
        public Task SetupStrategy(Message msg)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            if (msg.Text == "Back")
            {
                UserState[msg.Chat.Id] = MenuItem.GetStrategy;
                return client.SendTextMessageAsync(msg.Chat.Id,
                        "Выберите стратегию для настройки или введите название новой:", replyMarkup: AllStratsMenu(msg.Chat.Id));
            }
            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
            if (msg.Text == "Add new")
            {
                UserState[msg.Chat.Id] = MenuItem.AddIndicator;
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Indicator.ToList().ForEach(x => keyboardList.Add(new() { new KeyboardButton(x.Name) }));
                }
                return client.SendTextMessageAsync(msg.Chat.Id,
                    "Выберите индикатор для добавления:", replyMarkup: new ReplyKeyboardMarkup(keyboardList));
            }
            var strat = StrategyUserWorkingOn[userId];
            var txtSplit = msg.Text.Split(": ");
            var index = int.Parse(txtSplit[1]) - 1;
            using (ApplicationContext db = new ApplicationContext())
            {
                var indicators = db.Indicator.Where(x => x.Name == txtSplit[0]);
                if (indicators.Count() == 0)
                    return client.SendTextMessageAsync(msg.Chat.Id,
                        "Указанный индикатор не найден!");
                var indicator = indicators.First();
                var indics = db.StrategyIndicator.Where(x => x.StrategyId == strat.Id && x.IndicatorId == indicator.Id).OrderBy(x => x.Id).ToList();

                IndicatorUserWorkingOn[userId] = indics[index];
                UserState[msg.Chat.Id] = MenuItem.SetupField;
                var fields = db.InputField.Where(y => y.StrategyIndicatorId == indics[index].Id).ToList();
                foreach (var field in fields)
                {
                    db.Entry(field).Reference(x => x.IndicatorField).Load();
                    indics[index].InputFields.Add(field.IndicatorField.CodeName, field);
                    keyboardList.Add(new() { new KeyboardButton(field.IndicatorField.Name + ": " + field.IntValue) });
                }
                return client.SendTextMessageAsync(msg.Chat.Id,
                    "Выберите поле для настройки:", replyMarkup: new ReplyKeyboardMarkup(keyboardList));
            }
        }
        public Task AddIndicator(Message msg)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            if (msg.Text == "Back")
            {
                UserState[msg.Chat.Id] = MenuItem.SetupStrategy;
            }
            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
            using (ApplicationContext db = new ApplicationContext())
            {
                var indicator = db.Indicator.Where(x => x.Name == msg.Text).First();
                db.Entry(indicator).Collection(x => x.indicatorFields).Load();
                var strIndic = new StrategyIndicator();
                strIndic.IndicatorId = indicator.Id;
                strIndic.Indicator = indicator;
                strIndic.InputFields = new Dictionary<string, InputField>();
                strIndic.IsExit = false;
                strIndic.StrategyId = StrategyUserWorkingOn[userId].Id;
                db.StrategyIndicator.Add(strIndic);
                db.SaveChanges();

                indicator.indicatorFields.ForEach(x => 
                { 
                    if (x.IsInput == true) 
                    { 
                        keyboardList.Add(new() { new KeyboardButton(x.Name + ": " + 0) });
                        var inputField = new InputField();
                        inputField.IndicatorField = x;
                        inputField.IndicatorFieldId = x.Id;
                        inputField.StrategyIndicator = strIndic;
                        inputField.StrategyIndicatorId = strIndic.Id;
                        inputField.IntValue = 0;
                        inputField.FloatValue = 0;
                        strIndic.InputFields.Add(x.CodeName, inputField);
                        db.InputField.Add(inputField);
                    } 
                });
                db.SaveChanges();
                IndicatorUserWorkingOn[userId] = strIndic;
            }
            UserState[msg.Chat.Id] = MenuItem.SetupField;
            return client.SendTextMessageAsync(msg.Chat.Id,
                "Индикатор добавлен!\nВыберите поля для настройки:", replyMarkup: new ReplyKeyboardMarkup(keyboardList));
        }
        public Task SetupField(Message msg)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
            if (msg.Text == "Back")
            {
                UserState[msg.Chat.Id] = MenuItem.SetupStrategy;
                return client.SendTextMessageAsync(msg.Chat.Id, 
                    "Выберите индикатор для настройки или создайте новый:", replyMarkup: new ReplyKeyboardMarkup(GetKeyboardSetupStrategy(userId)));
            }
            var indicator = IndicatorUserWorkingOn[userId];
            using (ApplicationContext db = new ApplicationContext())
            {
                var allFields = db.InputField.Where(x => x.StrategyIndicatorId == indicator.Id).ToList();
                var name = msg.Text.Split(": ")[0];
                foreach(var field in allFields)
                {
                    db.Entry(field).Reference(x => x.IndicatorField).Load();
                    if(field.IndicatorField.Name == name)
                    {
                        FieldUserWorkingOn[userId] = field;
                        UserState[msg.Chat.Id] = MenuItem.SetupValue;
                        return client.SendTextMessageAsync(msg.Chat.Id,
                "Поле выбрано!\nВведите значение поля:", replyMarkup: new ReplyKeyboardMarkup(keyboardList));
                    }
                }
            }
            return client.SendTextMessageAsync(msg.Chat.Id,
                "Неверное наименование поля, попробуйте снова:");
        }
        public Task SetupValue(Message msg)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
            if (msg.Text == "Back")
            {
                UserState[msg.Chat.Id] = MenuItem.SetupField;
                client.SendTextMessageAsync(msg.Chat.Id,
                        "Неверное значение поля, попробуйте снова:", replyMarkup: new ReplyKeyboardMarkup(GetKeyboardSetupField(userId)));
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var field = FieldUserWorkingOn[userId];
                int intValue;
                float floatValue;
                if (int.TryParse(msg.Text!.Trim(), out intValue))
                {
                    field.IntValue = intValue;
                    field.FloatValue = null;
                }
                else if (float.TryParse(msg.Text!.Trim(), out floatValue))
                {
                    field.IntValue = null;
                    field.FloatValue = floatValue;
                }
                else
                {
                    return client.SendTextMessageAsync(msg.Chat.Id,
                        "Неверное значение поля, попробуйте снова:", replyMarkup: new ReplyKeyboardMarkup(keyboardList));
                }
                var indicator = IndicatorUserWorkingOn[userId];
                indicator.InputFields[field.IndicatorField.CodeName] = field;
                db.Update(field);
                db.SaveChanges();
            }
            UserState[msg.Chat.Id] = MenuItem.SetupField;
            return client.SendTextMessageAsync(msg.Chat.Id,
                        "Значение внесено:", replyMarkup: new ReplyKeyboardMarkup(GetKeyboardSetupField(userId)));
        }
        public List<List<KeyboardButton>> GetKeyboardSetupField(long userId)
        {
            var indic = IndicatorUserWorkingOn[userId];
            var result = new List<List<KeyboardButton>>();
            result.Add(new() { new KeyboardButton("Back") });
            foreach (var item in indic.InputFields)
            {
                result.Add(new() { new KeyboardButton(item.Value.IndicatorField.Name + ": " + item.Value.IntValue) });
            }
            return result;
        }
        public List<List<KeyboardButton>> GetKeyboardSetupStrategy(long userId)
        {
            var strat = StrategyUserWorkingOn[userId];
            var result = new List<List<KeyboardButton>>();
            result.Add(new() { new KeyboardButton("Back") });

            var cur = 1;
            var curName = "";
            foreach (var indic in strat.StrategyIndicators.OrderBy(x => x.Id))
            {
                if (curName == indic.Indicator.Name)
                {
                    cur++;
                }
                else
                {
                    curName = indic.Indicator.Name;
                    cur = 1;
                }
                result.Add(new() { new KeyboardButton(indic.Indicator.Name + ": " + cur) });
            }
            result.Add(new() { new KeyboardButton("Add new") });
            return result;
        }
        public Task TestStrategy(Message msg)
        {
            Strategy strat = null;
            var userId = Users.First(x => x.Value == msg.Chat.Id).Key;
            using (ApplicationContext db = new ApplicationContext())
            {
                strat = db.Strategy.Where(x => x.Name == msg.Text && x.AuthorId == userId).First();
                if (strat == null)
                {
                    return client.SendTextMessageAsync(msg.Chat.Id,
                        "Стратегии с таким названием не существует!");
                }
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
            Task result = Task.CompletedTask;
            if (picker != null)
            {
                try
                {
                    var file = picker.PickIndicators(strat, null);
                    using (Stream reader = System.IO.File.OpenRead(file))
                    {
                        client.SendDocumentAsync(msg.Chat.Id, InputFile.FromStream(stream: reader, fileName: "hamlet.csv"));
                    }
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Стратегия успешно протестирована!", replyMarkup: new ReplyKeyboardRemove());
                }
                catch (Exception ex)
                {
                    result = client.SendTextMessageAsync(msg.Chat.Id,
                        "Произошла какая-то ошибка!", replyMarkup: new ReplyKeyboardRemove());
                }
            }
            return result;
        }

        public IReplyMarkup TestStratMenu(long chatId)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            var userId = Users.First(x => x.Value == chatId).Key;
            using (ApplicationContext db = new ApplicationContext())
            {
                var accs = db.Account.Where(x => x.UserId == userId).ToList();
                foreach (var acc in accs)
                {
                    db.Entry(acc).Reference(x => x.Strategy).Load();
                    keyboardList.Add(new() { new KeyboardButton(acc.Strategy.Name) });
                }
            }

            return new ReplyKeyboardMarkup(keyboardList);
        }

        public IReplyMarkup AllStratsMenu(long chatId)
        {
            var keyboardList = new List<List<KeyboardButton>>();
            keyboardList.Add(new() { new KeyboardButton("Back") });
            var userId = Users.First(x => x.Value == chatId).Key;
            using (ApplicationContext db = new ApplicationContext())
            {
                var accs = db.Strategy.Where(x => x.AuthorId == userId && x.Name != null).ToList();
                foreach (var acc in accs)
                {
                    keyboardList.Add(new() { new KeyboardButton(acc.Name) });
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
            if (Users.ContainsValue(id))
            {
                client.SendTextMessageAsync(id,
                    message, parseMode: ParseMode.Html);
                return;
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
