using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E621TelegramBot.Conversations
{
    public class TestConversation : IConversationHandler
    {
        public async Task StartConversation(IConversationContext context)
        {
            await context.SendMessage("What is your name?");
            var message = await context.GetMessage();

            await context.SendMessage($"Hi there {message.Text}. What is your favourite color?");

            message = await context.GetMessage();

            await context.SendMessage($"I hate  {message.Text}.  Goodbye");
        }
    }
}
