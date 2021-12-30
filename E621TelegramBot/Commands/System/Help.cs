using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.System
{
    public class Help : IBotCommand
    {
        public string Command { get; } = "help";
        public string Description { get; } = "Shows a list of commands and help with using them";

        public Task Execute(Bot bot, TelegramBotClient botClient, Update update)
        {
            return botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                string.Join("\n", bot.Commands.Select(x => x.Command + " -- " + x.Description)));
        }
    }
}