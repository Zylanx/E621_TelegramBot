using System;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace E621Scraper
{
    public class Api
    {
        private const string UserAgent = "TeleBotTest/0.1 (by Zylinx on e621)";
        private const string BaseUrl = "https://e621.net/";
        private readonly string _username;
        private readonly string _apiKey;

        public Api(string username, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            _username = username;
            _apiKey = apiKey;

            FlurlHttp.Configure(settings =>
            {
                var resolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = resolver
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
            });
        }

        // TODO: Add handling of pages
        public Task<PostsCollection> ScrapeImages(string lastId)
        {
            return Request().AppendPathSegment("posts.json")
                            .SetQueryParams(new {limit = 370, page = $"a{lastId}"})
                            .GetJsonAsync<PostsCollection>();
        }

        public Task<PostsCollection> ScrapeImages()
        {
            return Request().AppendPathSegment("posts.json")
                            .SetQueryParam("limit", 30)
                            .GetJsonAsync<PostsCollection>();
        }

        private IFlurlRequest Request()
        {
            return BaseUrl.WithBasicAuth(_username, _apiKey)
                          .WithHeader("User-Agent", UserAgent);
        }
    }
}