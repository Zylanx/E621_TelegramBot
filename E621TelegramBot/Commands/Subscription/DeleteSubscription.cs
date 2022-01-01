using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class DeleteSubscription : BaseBotCommand
    {
        public DeleteSubscription()
        {
            Command = "delete";
            Description = "Delete a subscription";
        }

        public override Task Execute(ITelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }
    }
}