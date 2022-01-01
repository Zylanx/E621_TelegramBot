using System.Linq;
using System.Threading.Tasks;
using E621Shared.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class AddSubscription : BaseBotCommand
    {
        private readonly SubscriberRepo _subRepo;
        private readonly ILogger<AddSubscription> _logger;

        public AddSubscription(SubscriberRepo subRepo, ILogger<AddSubscription> logger)
        {
            Command = "add";
            Description = "Add a subscription";

            _subRepo = subRepo;
            _logger = logger;
        }

        public override async Task Execute(ITelegramBotClient botClient, Update update)
        {
            _logger.LogInformation(
                $"Processing command:\nFrom: {update.Message.From.Id}({update.Message.From.Username})\nText: {update.Message.Text}");
            // TODO: Support more kinds of messages e.g. channel
            var commandText = update.Message!.Text!.ToLower().Split(" ");

            string response;

            if (commandText.Length == 1)
            {
                response = "Invalid usage\nUsage: /add <E621 Tag>";
                _logger.LogInformation(
                    $"Invalid Command - Not enough arguments: {update.Message.From.Id}({update.Message.From.Username})");
            }
            else if (commandText.Length > 2)
            {
                response = "The command currently only supports adding one tag at a time"
                         + "\nTo add multiple subscriptions, send \"/add <tag>\" for each tag";
                _logger.LogInformation(
                    $"Invalid Command - Too many arguments: {update.Message.From.Id}({update.Message.From.Username})");
            }
            else
            {
                if ((await _subRepo.ListSubscriptionsForTelegramUser(update.Message!.From!.Id)).Any(subscription =>
                    subscription.Tag.ToLower() == commandText[1]))
                {
                    response = $"You have already added a subscription for \"{commandText[1]}\"";
                    _logger.LogInformation(
                        $"Invalid Command - Tag already subscribed: {update.Message.From.Id}({update.Message.From.Username})");
                }
                else
                {
                    // TODO: This should be limited to protect against dumb shit
                    await _subRepo.CreateSubscription(new E621Shared.Repositories.Subscription
                        {TelegramId = update.Message.From.Id, Tag = commandText[1]});

                    _logger.LogInformation(
                        $"Added Subscription for User {update.Message.From.Id}({update.Message.From.Username}): {commandText[1]}");

                    response = $"Subscription to \"{commandText[1]}\" added!";
                }
            }

            await botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                response);
        }
    }
}