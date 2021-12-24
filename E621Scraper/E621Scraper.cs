// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System;
using System.Linq;
using System.Threading.Tasks;

namespace E621Scraper
{
    public static class E621Scraper
    {
        public static async Task Main(string[] args)
        {
            Api api = new("", "");


            //Get initial set of results
            var results = (await api.ScrapeImages()).Posts;

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
                    await Task.Delay(4000);
                }

                results = await api.GetImagesSinceLastPoll(lastPollId);
                await Task.Delay(1000); //could have just made this one bigger
            }

            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey();
        }
    }
}