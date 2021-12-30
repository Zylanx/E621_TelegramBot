using System;
using System.Threading.Tasks;

namespace E621TelegramBot.Commands.Subscription
{
    public class DeleteSubscription : IBotCommand
    {
        public string Command { get; } = "delete";
        public string Description { get; } = "Delete a subscription";

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