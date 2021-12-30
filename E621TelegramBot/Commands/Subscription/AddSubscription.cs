using System;
using System.Threading.Tasks;

namespace E621TelegramBot.Commands.Subscription
{
    public class AddSubscription : IBotCommand
    {
        public string Command { get; } = "add";
        public string Description { get; } = "Add a subscription";

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