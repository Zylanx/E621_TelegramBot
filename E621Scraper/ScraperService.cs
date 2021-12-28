using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621Scraper.Configs;
using E621Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace E621Scraper
{
    public class ScraperService : IHostedService
    {
        private readonly Api.Api _api;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<ScraperService> _log;
        private readonly ScraperConfig _scraperConfig;
        private readonly ScraperRepo _scraperRepo;

        public ScraperService(IHostApplicationLifetime lifetime, Api.Api api, ScraperRepo scraperRepo,
                              ScraperConfig scraperConfig, ILogger<ScraperService> log)
        {
            _lifetime = lifetime;
            _api = api;
            _scraperRepo = scraperRepo;
            _scraperConfig = scraperConfig;
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
                }
                else
                {
                    _log.LogInformation("No new posts to display yet!");
                }

                try
                {
                    await Task.Delay((int)_scraperConfig.PollIntervalSeconds! * 1000, cancellationToken);
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
    }
}