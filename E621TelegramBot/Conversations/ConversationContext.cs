using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace E621TelegramBot.Conversations
{
    /// <summary>
    /// Represents a single conversation with a person
    /// </summary>
    public class ConversationContext : IConversationContext
    {
        private readonly long _telegramId;
        private readonly Func<ConversationMessage, Task> _sendMessageTask;
        private Queue<ConversationMessage> _messageQueue = new();
        private TaskCompletionSource<ConversationMessage>? _waitingTask = null;


        public ConversationContext(long telegramId, Func<ConversationMessage, Task> sendMessageTask)
        {
            this._telegramId = telegramId;
            this._sendMessageTask = sendMessageTask;
        }
        /// <summary>
        /// Returns a message from this conversation, if any, or blocks until there is one.
        /// </summary>
        /// <returns></returns>
        public Task<ConversationMessage> GetMessage()
        {
            if (_messageQueue.TryDequeue(out ConversationMessage? message))
            {
                //We have a message already
                return Task.FromResult(message);
            }
            else
            {
                //Block and wait for one.
                _waitingTask = new TaskCompletionSource<ConversationMessage>();
                return _waitingTask.Task;
            }

        }

        public void OnMessage(ConversationMessage message)
        {
            if (message.From != this._telegramId)
            {
                throw new ArgumentException("Message is not part of this converation");
            }

            if (_waitingTask != null)
            {
                _waitingTask.SetResult(message);
            }
            else
            {
                _messageQueue.Append(message);
            }
        }

        public Task SendMessage(ConversationMessage message)
        {
            message.To = this._telegramId;
            return _sendMessageTask(message);
        }
    }
}
