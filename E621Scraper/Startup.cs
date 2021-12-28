// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System.Threading.Tasks;
using E621Scraper.Configs;
using E621Shared;
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
        }

        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddHostedService<ScraperService>();
            services.AddSingleton<ConnectionProvider>();
            services.AddTransient(provider => provider.GetRequiredService<IConfiguration>().BindSection<ApiConfig>());
            services.AddTransient(_ => E621Shared.Config.DatabaseConfig);
            services.AddTransient(
                provider => provider.GetRequiredService<IConfiguration>().BindSection<ScraperConfig>());
            services.AddTransient<ScraperRepo>();
            services.AddTransient<Api.Api>();
        }
    }
}