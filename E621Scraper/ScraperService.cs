using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E621Scraper
{
    public class ScraperService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly Api _api;

        public ScraperService(Api api)
        {
            this._api = api;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Get initial set of results
            var results = (await _api.ScrapeImages()).Posts;

            var lastPollId = 0;

            while (true) //Simulate a rough polling loop, though we'll probably do this as a cronjob idk
            {
                if (results.Count > 0)
                {
                    lastPollId = results.Select(x => x.Id).OrderBy(x => x).Last();
                    Console.WriteLine(
                        $"Got {results.Count} images\nLast: {lastPollId}, Newest: {results.Last().Id}, Oldest: {results.First().Id}");
                }
                else
                {
                    Console.WriteLine($"No new posts to display yet! Last: {lastPollId}");
                }

                results = await _api.GetImagesSinceLastPoll(lastPollId);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                else
                {
                    await Task.Delay(5000); //could have just made this one bigger

                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
