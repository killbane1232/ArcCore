﻿using Arcam.Data;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;

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
        public static IPicker? picker = null;
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
                default:
                    if (!UserState.ContainsKey(msg.Chat.Id))
                    {
                        client.SendStickerAsync(msg.Chat.Id, InputFile.FromFileId("CAACAgIAAxkBAAIBmWFf8Ia0tHtyLUI9Pg2cfe2Pz87tAAIuAwACtXHaBqoozbmcyVK2IQQ"));
                        break;
                    }
                    switch (UserState[msg.Chat.Id]) 
                    {
                        case MenuItem.Start:
                            if(!UpdateUserTgId(msg.Text!, msg.Chat.Id))
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
            using (ApplicationContext db = new ApplicationContext())
            {
                var admins = db.User.Where(x => x.Access == 0).ToList();
                foreach (var item in admins)
                {
                    if (Users.ContainsValue(item.Id))
                    {
                        client.SendTextMessageAsync(Users[item.Id],
                            message);
                    }
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
