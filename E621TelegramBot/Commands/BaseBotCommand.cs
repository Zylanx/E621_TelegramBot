using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands
{
    public abstract class BaseBotCommand : IBotCommand
    {
        private readonly string _command = string.Empty;

        public string Command
        {
            get => _command;
            protected init => _command = "/" + value.TrimStart('/');
        }

        public string Description { get; protected init; } = "";

        public virtual bool Validate(Update update)
        {
            return update.Message?.Text?.Split(" ")[0].Equals(Command) ?? false;
        }

        public abstract Task Execute(ITelegramBotClient botClient, Update update);
    }
}