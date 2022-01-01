// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System;
using System.Threading.Tasks;
using E621Scraper.Configs;
using E621Shared;
using E621TelegramBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace E621Scraper
{
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
            services.AddHostedService<ScraperService>()
                    .AddTransient<ConnectionProvider>()
                    .AddConfig<BotConfig>()
                    .AddConfig<ApiConfig>()
                    .AddConfig(E621Shared.Config.DatabaseConfig)
                    .AddConfig<ScraperConfig>()
                    .AddTransient<ScraperRepo>()
                    .AddTransient<Api.Api>()
                    .AddTransient<SubscriberRepo>();
        }
    }
}