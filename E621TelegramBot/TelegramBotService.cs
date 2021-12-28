using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using E621Shared;

namespace E621TelegramBot
{
    public class TelegramBotService : IHostedService
    {
        private readonly ScraperRepo _scraperRepo;
        private readonly ILogger<TelegramBotService> _log;

        public TelegramBotService(ScraperRepo scraperRepo, ILogger<TelegramBotService> log)
        {
            _scraperRepo = scraperRepo;
            this._log = log;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Bot starting");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
