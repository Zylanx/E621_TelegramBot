using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621Scraper.Api;
using E621Scraper.Configs;
using E621Shared;
using E621TelegramBot.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace E621Scraper
{
    public class ScraperService : IHostedService
    {
        private const int PostChunkSize = 10;
        private const int TagChunkSize = 50;
        
        private readonly Api.Api _api;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<ScraperService> _log;
        private readonly ScraperConfig _scraperConfig;
        private readonly ScraperRepo _scraperRepo;
        private readonly SubscriberRepo _subRepo;
        private readonly TelegramBotClient _botClient;

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
                    ProcessPosts(results);
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
            foreach (var subscriptionsChunk in (await _subRepo.ListAllSubscriptionsForTags(post.AllTags)).Chunk(
                TagChunkSize))
            {
                var tasks = subscriptionsChunk.Select(subscription =>
                {
                    _log.LogDebug(
                        $"Processing Subscription {subscription.TelegramId} {subscription.Tag} for tag {subscription.Tag} on post {post.Id}");
                    return _botClient.SendTextMessageAsync(subscription.TelegramId,
                        $"https://e621.net/posts/{post.Id}");
                });

                await Task.WhenAll(tasks);
            }
        }

        private async Task ProcessPosts(List<Post> posts)
        {
            foreach (var postsChunk in posts.Chunk(PostChunkSize))
            {
                var tasks = postsChunk.Select(post =>
                {
                    _log.LogDebug($"Processing post {post.Id}");
                    return PropagatePost(post);
                });
                await Task.WhenAll(tasks);
            }
        }
    }
}