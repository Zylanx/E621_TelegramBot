using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Telegram
{
    public class Start : IBotCommand
    {
        public string Command { get; } = "start";
        public string Description { get; } = "Starts the bot";

        public Task Execute(ITelegramBotClient botClient, Update update)
        {
            return botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                "Welcome to this E621 bot\ntype /help to see the commands");
        }
    }
}