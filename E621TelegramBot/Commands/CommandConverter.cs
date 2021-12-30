using Telegram.Bot.Types;

namespace E621TelegramBot.Commands
{
    public static class CommandConverter
    {
        public static BotCommand ToBotCommand(this IBotCommand command)
        {
            return new BotCommand {Command = command.Command, Description = command.Description};
        }
    }
}