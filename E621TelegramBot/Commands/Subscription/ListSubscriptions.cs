using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class ListSubscriptions : IBotCommand
    {
        public string Command { get; } = "list";
        public string Description { get; } = "List all subscriptions";

        public Task Execute(Bot bot, TelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }
    }
}