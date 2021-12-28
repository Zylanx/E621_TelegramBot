// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using System.Threading.Tasks;
using E621Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace E621Scraper
{
    public static class Startup
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                      .ConfigureServices(ConfigureServices)
                      .RunConsoleAsync();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();
            
            services.AddHostedService<ScraperService>();
            services.AddSingleton<ConnectionProvider>();
            services.AddTransient(_ => configuration.GetRequiredSection("ApiConfig").Get<ApiConfig>());
            services.AddTransient(_ => E621Shared.Config.DatabaseConfig);
            services.AddTransient<ScraperRepo>();
            services.AddTransient<Api>();
        }
    }
}