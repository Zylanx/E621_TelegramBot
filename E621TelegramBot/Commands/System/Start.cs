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

        public Task Execute(ITelegramBotClient botClient, Update update)
        {
            //Don't really need this command as its stateless right now
            /* var welcome = botClient.SendTextMessageAsync(
                 update.Message!.Chat.Id,
                 "Welcome to the bot, commands are");

             //var help = bot.Commands.Where(command => command.Command == "help").First().Execute(bot, botClient, update);

             await Task.WhenAll(welcome, help);
            */
            return Task.CompletedTask;
        }
    }
}