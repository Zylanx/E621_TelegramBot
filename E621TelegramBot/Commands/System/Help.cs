using System;
using System.Threading.Tasks;

namespace E621TelegramBot.Commands.System
{
    public class Help : IBotCommand
    {
        public string Command { get; } = "help";
        public string Description { get; } = "Shows a list of commands and help with using them";

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