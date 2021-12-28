// This is a janky test, not intended to work well or be well documented.
// Don't expect quality code.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            var apiConfig = configuration.GetValue<ApiConfig>("ApiConfig");

            services.AddHostedService<ScraperService>();
            services.AddTransient(x => configuration.GetRequiredSection("ApiConfig").Get<ApiConfig>());
            services.AddTransient<Api>();
        }
    }
}