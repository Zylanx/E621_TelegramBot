using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E621TelegramBot.Conversations
{
    public interface IConversationContext
    {
        public Task<ConversationMessage> GetMessage();
        public Task SendMessage(ConversationMessage message);

    }



    public static class ConversationExtensions
    {
        public static Task SendMessage(this IConversationContext context, string text)
        {
            return context.SendMessage(new ConversationMessage() { Text = text });
        }
    }
}
