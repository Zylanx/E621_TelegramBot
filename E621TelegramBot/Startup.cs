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
        private static IConfigurationRoot? Configuration { get; set; }
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
            Configuration = builder.Build();

            await Host.CreateDefaultBuilder(args)
                      .ConfigureServices(ConfigureServices)
                      .ConfigureLogging(ConfigureLogging)
                      .RunConsoleAsync();
        }


        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            services.AddHostedService<TelegramBotService>();
            services.AddSingleton<ConnectionProvider>();
            services.AddTransient(_ => Config.DatabaseConfig);
            services.AddTransient(_ => Configuration.GetRequiredSection("BotConfig").Get<BotConfig>());
            services.AddTransient<ScraperRepo>();
        }
    }
}