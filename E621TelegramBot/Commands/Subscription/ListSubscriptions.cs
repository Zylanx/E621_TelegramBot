using System.Linq;
using System.Threading.Tasks;
using E621Shared;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace E621TelegramBot.Commands.Subscription
{
    public class ListSubscriptions : BaseBotCommand
    {
        private readonly SubscriberRepo _subRepo;

        public ListSubscriptions(SubscriberRepo subRepo)
        {
            Command = "list";
            Description = "List all your subscriptions";

            _subRepo = subRepo;
        }

        public override async Task Execute(ITelegramBotClient botClient, Update update)
        {
            var subList = await _subRepo.ListSubscriptionsForTelegramUser(update.Message!.From!.Id);
            string response = string.Join("\n", subList.Select((subscription, i) => $"{i + 1}. {subscription.Tag}"));

            if (string.IsNullOrWhiteSpace(response))
            {
                response = "No subscriptions to list.";
            }

            await botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                response);
        }
    }
}