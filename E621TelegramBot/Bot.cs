using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621TelegramBot.Conversations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace E621TelegramBot
{
    public class Bot
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<TelegramBotClient> _log;
        private readonly BotConfig _config;
        private readonly IMemoryCache _conversationCache;

        public Bot(ILogger<TelegramBotClient> log, BotConfig config, IMemoryCache conversationCache)
        {
            //todo: read from botconfig.
            _log = log;
            _config = config;
            this._conversationCache = conversationCache;
            _botClient = new TelegramBotClient(config.ApiKey);

            Commands = new List<BotCommand>();
            Commands.Add(new BotCommand { Command = "start", Description = "Get started" });
            Commands.Add(new BotCommand { Command = "help", Description = "Receive help" });
        }

        public List<BotCommand> Commands { get; }

        public async Task StartListening(CancellationToken cancellationToken)
        {
            // TODO: Handle cancellation token

            var receiverOptions = new ReceiverOptions();
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken);

            var me = await _botClient.GetMeAsync(cancellationToken);
            _log.LogInformation($"Start listening for @{me.Username}");

            _log.LogDebug("Sending commands");
            await _botClient.SetMyCommandsAsync(Commands, cancellationToken: cancellationToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
                                             CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            //Check if a conversation with this person exists or not.
            var message = new ConversationMessage() { From = update.Message.From.Id, Text = update.Message.Text };
            var cacheKey = $"Conversation{message.From}";
            if (_conversationCache.TryGetValue(cacheKey, out ConversationContext value))
            {
                value.OnMessage(message);
            }
            else
            {
                var converationContext = new ConversationContext(message.From, x => botClient.SendTextMessageAsync(x.To, x.Text, cancellationToken: cancellationToken));
                var conversation = new TestConversation();
                var task = conversation.StartConversation(converationContext);
                _conversationCache.Set(cacheKey, converationContext, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(30) });
            }



            var chatId = update.Message.Chat.Id;

            string help = string.Join("\r\n", Commands.Select(x => x.Command + " -- " + x.Description));

            //todo: proccess update.Message for commands in command list
            /*
            await botClient.SendTextMessageAsync(
                chatId,6
                "Welcome to the bot, commands are",
                cancellationToken: cancellationToken);

            await botClient.SendTextMessageAsync(
                chatId,
                help,
                cancellationToken: cancellationToken);
            */
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
                                      CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _log.LogError(errorMessage);
            return Task.CompletedTask;
        }
    }
}