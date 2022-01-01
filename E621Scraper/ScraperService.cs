using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621Scraper.Api;
using E621Scraper.Configs;
using E621Shared.Configs;
using E621Shared.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace E621Scraper
{
    public class ScraperService : IHostedService
    {
        // private const int PostChunkSize = 50;
        // private const int TagChunkSize = 200;

        private readonly Api.Api _api;
        private readonly TelegramBotClient _botClient;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<ScraperService> _log;
        private readonly ScraperConfig _scraperConfig;
        private readonly ScraperRepo _scraperRepo;
        private readonly SubscriberRepo _subRepo;

        public ScraperService(IHostApplicationLifetime lifetime, Api.Api api, ScraperRepo scraperRepo,
                              BotConfig botConfig, SubscriberRepo subRepo, ScraperConfig scraperConfig,
                              ILogger<ScraperService> log)
        {
            _lifetime = lifetime;
            _api = api;
            _scraperRepo = scraperRepo;
            _subRepo = subRepo;
            _scraperConfig = scraperConfig;
            _botClient = new TelegramBotClient(botConfig.ApiKey);
            _log = log;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () => { await ScrapeImages(cancellationToken); }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ScrapeImages(CancellationToken cancellationToken)
        {
            while (true) //Simulate a rough polling loop, though we'll probably do this as a cronjob idk
            {
                var lastPollId = await _scraperRepo.GetLastPolledId();
                _log.LogDebug($"Getting images using last: {lastPollId}");
                var results = await _api.GetImagesSinceLastPoll(lastPollId, cancellationToken);

                if (results.Count > 0)
                {
                    await _scraperRepo.UpdateLastPolledId(results.Select(x => x.Id).OrderBy(x => x).Last());
                    _log.LogInformation(
                        $"Got {results.Count} images\nLast: {await _scraperRepo.GetLastPolledId()}, Newest: {results.Last().Id}, Oldest: {results.First().Id}");
                    await ProcessPosts(results);
                }
                else
                {
                    _log.LogInformation("No new posts to display yet!");
                }

                try
                {
                    await Task.Delay(_scraperConfig.PollInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    ;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private async Task PropagatePost(Post post)
        {
            // foreach (var subscriptionsChunk in (await _subRepo.ListAllSubscriptionsForTags(post.AllTags)).Chunk(
            //     TagChunkSize))
            //     await Task.WhenAll((await _subRepo.ListAllSubscriptionsForTags(post.AllTags)).Select(subscription =>
            //     {
            //         var tasks = subscriptionsChunk.Select(subscription =>
            //         {
            //             _log.LogDebug(
            //                 $"Processing Subscription {subscription.TelegramId} for tag {subscription.Tag} on post {post.Id}");
            //             return _botClient.SendTextMessageAsync(subscription.TelegramId,
            //                 $"https://e621.net/posts/{post.Id}");
            //         });
            //
            //         await Task.WhenAll(tasks);
            //     }

            await Task.WhenAll((await _subRepo.ListAllSubscriptionsForTags(post.AllTags)).Select(subscription =>
            {
                _log.LogDebug(
                    $"Processing Subscription {subscription.TelegramId} for tag {subscription.Tag} on post {post.Id}");
                return _botClient.SendTextMessageAsync(subscription.TelegramId,
                    $"https://e621.net/posts/{post.Id}");
            }));
        }

        private async Task ProcessPosts(List<Post> posts)
        {
            // foreach (var postsChunk in posts.Chunk(PostChunkSize))
            // {
            //     var tasks = postsChunk.Select(post =>
            //     {
            //         _log.LogDebug($"Processing post {post.Id}");
            //         return PropagatePost(post);
            //     });
            //     await Task.WhenAll(tasks);
            // }

            await Task.WhenAll(posts.Select(post => PropagatePost(post)));
        }
    }
}