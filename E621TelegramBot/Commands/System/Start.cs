using System;
using System.Threading.Tasks;

namespace E621TelegramBot.Commands.System
{
    public class Start : IBotCommand
    {
        public string Command { get; } = "start";
        public string Description { get; } = "Starts the bot";

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