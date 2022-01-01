using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Telegram
{
    public class Start : BaseBotCommand
    {
        public Start()
        {
            Command = "start";
            Description = "Starts the bot";
        }

        public override Task Execute(ITelegramBotClient botClient, Update update)
        {
            return botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                "Welcome to this E621 bot\ntype /help to see the commands");
        }
    }
}