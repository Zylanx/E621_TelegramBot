using System;
using System.Threading.Tasks;

namespace E621TelegramBot.Commands.Subscription
{
    public class ListSubscriptions : IBotCommand
    {
        public string Command { get; } = "list";
        public string Description { get; } = "List all subscriptions";

        public Task<bool> Validate()
        {
            throw new NotImplementedException();
        }

        public Task Execute()
        {
            throw new NotImplementedException();
        }
    }
}