using System;
using System.Threading.Tasks;
using E621Shared;

namespace E621TelegramBot.Commands.Subscription
{
    public class AddSubscription : IBotCommand
    {
        private UserRepo _userRepo;
        
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