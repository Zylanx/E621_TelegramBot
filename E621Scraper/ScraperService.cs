using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E621Shared;
using Microsoft.Extensions.Hosting;

namespace E621Scraper
{
    public class ScraperService : IHostedService
    {
        private readonly Api _api;
        private readonly ScraperRepo _scraperRepo;

        public ScraperService(Api api, ScraperRepo scraperRepo)
        {
            _api = api;
            _scraperRepo = scraperRepo;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Should this be registering on ApplicationStarted or is this fine?
            while (true) //Simulate a rough polling loop, though we'll probably do this as a cronjob idk
            {
                var results = await _api.GetImagesSinceLastPoll(await _scraperRepo.GetLastPolledId());

                if (results.Count > 0)
                {
                    await _scraperRepo.UpdateLastPolledId(results.Select(x => x.Id).OrderBy(x => x).Last());
#if DEBUG
                    Console.WriteLine(
                        $"Got {results.Count} images\nLast: {await _scraperRepo.GetLastPolledId()}, Newest: {results.Last().Id}, Oldest: {results.First().Id}");
#endif
                }
                else
                {
                    Console.WriteLine("No new posts to display yet!");
#if DEBUG
                    Console.WriteLine("Last: {lastPollId}");
#endif
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

#if DEBUG
                // TODO: Look into adding better cancellation support
                await Task.Delay(5000);
#else
                await Task.Delay(TimeSpan.FromMinutes(1));
#endif
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}