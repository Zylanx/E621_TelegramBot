using System;

namespace E621Scraper.Configs
{
    public class ScraperConfig
    {
        public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(30);
    }
}