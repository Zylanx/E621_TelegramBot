using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Telegram
{
    public class Help : IBotCommand
    {
        private readonly List<IBotCommand> _commands;

        public string Command { get; } = "help";
        public string Description { get; } = "Shows a list of commands and help with using them";


        public Help(List<IBotCommand> commands)
        {
            this._commands = commands;
        }

        public Task Execute(ITelegramBotClient botClient, Update update)
        {
            return botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                "The available commands are:\n" +
                string.Join("\n", _commands.Select(x => x.Command + " -- " + x.Description)));
        }
    }
}