﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public List<BotCommand> _commands { get; }

        public Bot(ILogger<TelegramBotClient> log)
        {
            //todo: read from botconfig.
            _botClient = new TelegramBotClient("");
            this._log = log;

            _commands = new List<BotCommand>();
            _commands.Add(new BotCommand() { Command = "start", Description = "Get started" });
            _commands.Add(new BotCommand() { Command = "fuck", Description = "Make the bot fuck" });
        }

        public async Task StartListening(CancellationToken cancellationToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };
            _botClient.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken: cancellationToken);

            var me = await _botClient.GetMeAsync();
            _log.LogInformation($"Start listening for @{me.Username}");

            _log.LogDebug("Sending commands");
            await _botClient.SetMyCommandsAsync(_commands);

        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;

            string help = string.Join("\r\n", _commands.Select(x => x.Command + " -- " + x.Description));

            //todo: proccess update.Message for commands in command list

            await botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: "Welcome to the bot, commands are",
                   cancellationToken: cancellationToken);

            await botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: help,
                   cancellationToken: cancellationToken);
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _log.LogError(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}