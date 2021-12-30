using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621TelegramBot.Configuration;
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

        public Bot(ILogger<TelegramBotClient> log, BotConfig config)
        {
            _log = log;
            _config = config;
            _botClient = new TelegramBotClient(_config.ApiKey);

            Commands = new List<BotCommand>();
            Commands.Add(new BotCommand {Command = "start", Description = "Get started"});
            Commands.Add(new BotCommand {Command = "help", Description = "Receive help"});
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

            var chatId = update.Message.Chat.Id;

            string help = string.Join("\r\n", Commands.Select(x => x.Command + " -- " + x.Description));

            //todo: proccess update.Message for commands in command list

            await botClient.SendTextMessageAsync(
                chatId,
                "Welcome to the bot, commands are",
                cancellationToken: cancellationToken);

            await botClient.SendTextMessageAsync(
                chatId,
                help,
                cancellationToken: cancellationToken);
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