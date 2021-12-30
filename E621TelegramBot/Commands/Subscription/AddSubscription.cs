using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class AddSubscription : IBotCommand
    {
        public string Command { get; } = "add";
        public string Description { get; } = "Add a subscription";

        public Task Execute(Bot bot, TelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }
    }
}