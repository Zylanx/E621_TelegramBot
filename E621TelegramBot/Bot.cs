using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621TelegramBot.Commands;
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
        private readonly BotConfig _config;
        private readonly ILogger<TelegramBotClient> _logger;

        public Bot(ILogger<TelegramBotClient> logger, BotConfig config, IEnumerable<IBotCommand> commands)
        {
            _logger = logger;
            _config = config;
            Commands = commands.ToList();
            _botClient = new TelegramBotClient(_config.ApiKey);
        }

        public List<IBotCommand> Commands { get; }

        public async Task StartListening(CancellationToken cancellationToken)
        {
            // TODO: Handle cancellation token

            var receiverOptions = new ReceiverOptions();
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken);

            _logger.LogDebug("Sending commands");
            await _botClient.SetMyCommandsAsync(Commands.Select(command => command.ToBotCommand()),
                cancellationToken: cancellationToken);

            var me = await _botClient.GetMeAsync(cancellationToken);
            _logger.LogInformation($"Start listening for @{me.Username}");
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

            //todo: proccess update.Message for commands in command list

            foreach (var command in Commands)
            {
                try
                {
                    if (command.Validate(update))
                    {
                        try
                        {
                            await command.Execute(this, _botClient, update);
                            return;
                        }
                        catch (NotImplementedException e)
                        {
                            await HandleErrorAsync(botClient, e, cancellationToken);
                            return;
                        }
                    }
                }
                catch (NotImplementedException)
                {
                }
                catch (Exception e)
                {
                    await HandleErrorAsync(botClient, e, cancellationToken);
                }
            }

            // TODO: Give this an actual exception class
            await HandleErrorAsync(botClient, new InvalidOperationException("No command found for message"),
                cancellationToken);
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

            _logger.LogError(errorMessage);
            return Task.CompletedTask;
        }
    }
}