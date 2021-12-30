// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System.Threading.Tasks;
using E621Shared;
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
                      .ConfigureServices(ConfigureServices)
                      .ConfigureLogging(ConfigureLogging)
                      .RunConsoleAsync();
        }


        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.ClearProviders()
                   .AddConsole();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddHostedService<TelegramBotHost>()
                    .AddSingleton<ConnectionProvider>()
                    .AddSingleton<Bot>()
                    .AddConfig(Config.DatabaseConfig)
                    .AddConfig<BotConfig>()
                    .AddTransient<ScraperRepo>();
        }
    }
}