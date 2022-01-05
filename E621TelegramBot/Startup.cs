// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System;
using System.Threading.Tasks;
using E621Shared;
using E621Shared.Configs;
using E621Shared.Repositories;
using E621TelegramBot.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace E621TelegramBot
{
    public static class Startup
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                      .ConfigureAppConfiguration(configurationBuilder =>
                          configurationBuilder.AddJsonFile("appsettings.json", false, true))
                      .ConfigureLogging(ConfigureLogging)
                      .ConfigureServices(ConfigureServices)
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
            services.AddConfig(Config.DatabaseConfig)
                    .AddConfig<BotConfig>()
                    .AddScoped<ScraperRepo>()
                    .AddScoped<SubscriberRepo>()
                    .AddBotCommands()
                    .AddHostedService<TelegramBotHost>()
                    .AddScoped<ConnectionProvider>()
                    .AddSingleton<Bot>();
        }
    }
}