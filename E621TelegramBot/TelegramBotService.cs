using System.Threading;
using System.Threading.Tasks;
using E621Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace E621TelegramBot
{
    public class TelegramBotService : IHostedService
    {
        private readonly ILogger<TelegramBotService> _log;
        private readonly ScraperRepo _scraperRepo;

        public TelegramBotService(ScraperRepo scraperRepo, ILogger<TelegramBotService> log)
        {
            _scraperRepo = scraperRepo;
            _log = log;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Bot starting");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Bot stopped");
            return Task.CompletedTask;
        }
    }
}