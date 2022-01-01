using System.Linq;
using System.Threading.Tasks;
using E621Shared.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class DeleteSubscription : BaseBotCommand
    {
        private readonly ILogger<DeleteSubscription> _logger;
        private readonly SubscriberRepo _subRepo;

        public DeleteSubscription(ILogger<DeleteSubscription> logger, SubscriberRepo subRepo)
        {
            Command = "delete";
            Description = "Delete a subscription";

            _logger = logger;
            _subRepo = subRepo;
        }

        public override async Task Execute(ITelegramBotClient botClient, Update update)
        {
            _logger.LogDebug(
                $"Processing command - From: {update.Message.From.Id}({update.Message.From.Username}), Text: {update.Message.Text}");
            // TODO: Support more kinds of messages e.g. channel
            var commandText = update.Message!.Text!.ToLower().Split(" ");

            string response;

            if (commandText.Length == 1)
            {
                response = "Invalid usage\nUsage: /delete <E621 Tag>";
                _logger.LogDebug(
                    $"Invalid Command - Not enough arguments: {update.Message.From.Id}({update.Message.From.Username})");
            }
            else if (commandText.Length > 2)
            {
                response = "The command currently only supports deleting one subscription/tag at a time"
                         + "\nTo delete multiple subscriptions, send \"/delete <tag>\" for each tag";
                _logger.LogDebug(
                    $"Invalid Command - Too many arguments: {update.Message.From.Id}({update.Message.From.Username})");
            }
            else
            {
                if ((await _subRepo.ListSubscriptionsForTelegramUser(update.Message!.From!.Id)).Any(subscription =>
                    subscription.Tag.ToLower() == commandText[1]))
                {
                    // TODO: This should be limited to protect against dumb shit
                    await _subRepo.DeleteSubscription(update.Message!.From!.Id, commandText[1]);

                    _logger.LogInformation(
                        $"Removed Subscription for User {update.Message.From.Id}({update.Message.From.Username}): {commandText[1]}");

                    response = $"Subscription to \"{commandText[1]}\" deleted!";
                }
                else
                {
                    response = $"You do not have a subscription to \"{commandText[1]}\"";
                    _logger.LogDebug(
                        $"Invalid Command - Tag not subscribed: {update.Message.From.Id}({update.Message.From.Username})");
                }
            }

            await botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                response);
        }
    }
}