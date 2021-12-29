using System;
using System.Threading;
using System.Threading.Tasks;
using E621Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace E621TelegramBot
{
    public class TelegramBotHost : IHostedService
    {
        private readonly ILogger<TelegramBotHost> _log;
        private readonly Bot _bot;
        private readonly ScraperRepo _scraperRepo;

        public TelegramBotHost(ScraperRepo scraperRepo, ILogger<TelegramBotHost> log, Bot bot)
        {
            _scraperRepo = scraperRepo;
            _log = log;
            this._bot = bot;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Bot starting");
            await _bot.StartListening(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Bot stopped");
            return Task.CompletedTask;
        }
    }
}