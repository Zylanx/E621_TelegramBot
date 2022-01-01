using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class ListSubscriptions : BaseBotCommand
    {
        public ListSubscriptions()
        {
            Command = "list";
            Description = "List all your subscriptions";
        }

        public override Task Execute(ITelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }
    }
}