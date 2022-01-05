using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using E621Shared.Configs;
using E621TelegramBot.Commands;
using E621TelegramBot.Commands.Telegram;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TelegramBotClient> _logger;

        public Bot(ILogger<TelegramBotClient> logger, BotConfig config, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(config.ApiKey))
            {
                throw new ArgumentException("Must configure an API key");
            }
            serviceProvider.CreateScope();
            _logger = logger;
            _config = config;
            this._serviceProvider = serviceProvider;
            _botClient = new TelegramBotClient(_config.ApiKey);
        }

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
            // await _botClient.SetMyCommandsAsync(Commands.Select(command => command.ToBotCommand()),
            // cancellationToken: cancellationToken);

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

            try
            {
                using (IServiceScope serviceScope = _serviceProvider.CreateScope())
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    await HandleUpdateAsyncInternal(serviceScope, botClient, update, cancellationToken);
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }


        private Task HandleUpdateAsyncInternal(IServiceScope serviceScope, ITelegramBotClient botClient, Update update,
                                               CancellationToken cancellationToken)
        {
            var commands = serviceScope.ServiceProvider.GetRequiredService<IEnumerable<IBotCommand>>().ToList();
            var helpCommand = new Help(commands);
            var command = commands.FirstOrDefault(x => x.Validate(update));

            var task = command switch
            {
                null => helpCommand.Execute(botClient, update),
                var commanda => commanda.Execute(botClient, update)
            };

            return task;
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