using System.Threading.Tasks;

namespace E621TelegramBot.Conversations
{
    public interface IConversationHandler
    {
        public Task StartConversation(IConversationContext context);
    }
}
