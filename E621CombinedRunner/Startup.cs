// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System;
using System.Threading.Tasks;
using E621Scraper;
using E621Scraper.Api;
using E621Scraper.Configs;
using E621Shared;
using E621Shared.Configs;
using E621Shared.Repositories;
using E621TelegramBot;
using E621TelegramBot.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Config = E621Shared.Config;

namespace E621CombinedRunner
{
    // TODO: Add a global blacklist
    
    public static class Startup
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                      .ConfigureAppConfiguration(configurationBuilder =>
                          configurationBuilder.AddJsonFile("appsettings.json", false, true))
                      .ConfigureServices(ConfigureServices)
                      .ConfigureLogging(ConfigureLogging)
                      .RunConsoleAsync();

            Console.Write("Press any key to continue . . .");
            Console.ReadKey();
        }

        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.ClearProviders()
                   .AddConsole();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddConfig<BotConfig>()
                    .AddConfig<ApiConfig>()
                    .AddConfig(Config.DatabaseConfig)
                    .AddConfig<ScraperConfig>()
                    .AddSingleton<ConnectionProvider>()
                    .AddTransient<ScraperRepo>()
                    .AddTransient<SubscriberRepo>()
                    .AddTransient<Api>()
                    .AddBotCommands()
                    .AddTransient<Bot>()
                    .AddHostedService<ScraperService>()
                    .AddHostedService<TelegramBotHost>();
        }
    }
}