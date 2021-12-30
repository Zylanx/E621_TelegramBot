using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.System
{
    public class Start : IBotCommand
    {
        public string Command { get; } = "start";
        public string Description { get; } = "Starts the bot";

        public async Task Execute(Bot bot, TelegramBotClient botClient, Update update)
        {
            var welcome = botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                "Welcome to the bot, commands are");

            var help = bot.Commands.Where(command => command.Command == "help").First().Execute(bot, botClient, update);

            await Task.WhenAll(welcome, help);
        }
    }
}