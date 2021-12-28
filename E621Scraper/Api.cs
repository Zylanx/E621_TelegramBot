using System;
using System.Collections.Generic;
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
        private const int MaxPostsPerRequest = 320;
        private readonly ApiConfig _config;

        public Api(ApiConfig config)
        {
            if(config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrWhiteSpace(config.Username))
            {
                throw new ArgumentNullException(nameof(config.Username));
            }

            if (string.IsNullOrWhiteSpace(config.Password))
            {
                throw new ArgumentNullException(nameof(config.Password));
            }

            this._config = config;

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

        private Task<PostsCollection> ScrapeImagesBeforeId(int? id)
        {
            if (id == null)
            {
                return ScrapeImages();
            }

            return Request().AppendPathSegment("posts.json")
                            .SetQueryParams(new {limit = MaxPostsPerRequest, page = $"b{id}"})
                            .GetJsonAsync<PostsCollection>();
        }

        // TODO: add a global ratelimit timer so that it is impossible to get limited
        private Task<PostsCollection> ScrapeImagesAfterId(int? lastId)
        {
            if (lastId == null)
            {
                return ScrapeImages();
            }

            return Request().AppendPathSegment("posts.json")
                            .SetQueryParams(new {limit = MaxPostsPerRequest, page = $"a{lastId}"})
                            .GetJsonAsync<PostsCollection>();
        }

        public Task<PostsCollection> ScrapeImages()
        {
            return Request().AppendPathSegment("posts.json")
                            .SetQueryParam("limit", MaxPostsPerRequest)
                            .SetQueryParam("page", 3)
                            .GetJsonAsync<PostsCollection>();
        }


        //Get all pages from last time we polled otherwise just get max pages.
        //The trick here is we have to keep fetching until we see lastPollId in the list then stop and remove any shit smaller than that.
        public async Task<List<Post>> GetImagesSinceLastPoll(int lastPollId)
        {
            List<Post> results = new();
            while (
                true) // I think this logic might still be a bit fucked, lets fuck off this nullable lastpollid shit and just do an upfront call seperate
            {
                var pages = (await ScrapeImagesAfterId(lastPollId)).Posts; //Gets posts in oldest to newest order.

                if (pages.Count == 0)
                {
                    //No posts, bail right away.
                    break;
                }

                await Task.Delay(1000);

                results.AddRange(pages);

                lastPollId = pages[0].Id; //Get the next newest batch, keep going until there is no newer posts.
            }

            return results; //done.
        }

        private IFlurlRequest Request()
        {
            return BaseUrl.WithBasicAuth(_config.Username, _config.Password)
                          .WithHeader("User-Agent", UserAgent);
        }
    }
}