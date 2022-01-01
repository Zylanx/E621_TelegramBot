using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Telegram
{
    public class Help : BaseBotCommand
    {
        private readonly List<IBotCommand> _commands;

        public Help(List<IBotCommand> commands)
        {
            Command = "help";
            Description = "Shows a list of commands and help with using them";
            _commands = commands;
        }

        // TODO: Add individual command help
        public override Task Execute(ITelegramBotClient botClient, Update update)
        {
            return botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                "The available commands are:\n" +
                string.Join("\n", _commands.Select(x => x.Command + " -- " + x.Description)));
        }
    }
}