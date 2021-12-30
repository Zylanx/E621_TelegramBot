using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class DeleteSubscription : IBotCommand
    {
        public string Command { get; } = "delete";
        public string Description { get; } = "Delete a subscription";

        public Task Execute(Bot bot, TelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }
    }
}