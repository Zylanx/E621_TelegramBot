using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E621Scraper.Configs;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace E621Scraper.Api
{
    public class Api
    {
        private const string BaseUrl = "https://e621.net/";
        private readonly ApiConfig _config;

        public Api(ApiConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

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

        private async Task<PostsCollection> ScrapeImagesBeforeId(int? id)
        {
            if (id == null)
            {
                return await ScrapeImages();
            }

            await Task.Delay(1000);

            return await Request().AppendPathSegment("posts.json")
                                  .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = $"b{id}"})
                                  .GetJsonAsync<PostsCollection>();
        }

        // TODO: add a global ratelimit timer so that it is impossible to get limited
        private async Task<PostsCollection> ScrapeImagesAfterId(int? lastId)
        {
            if (lastId == null)
            {
                return await ScrapeImages();
            }

            await Task.Delay(1000);

            return await Request().AppendPathSegment("posts.json")
                                  .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = $"a{lastId}"})
                                  .GetJsonAsync<PostsCollection>();
        }

        private async Task<PostsCollection> ScrapeImages()
        {
            await Task.Delay(1000);

            return await Request().AppendPathSegment("posts.json")
                                  .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = "2"})
                                  .GetJsonAsync<PostsCollection>();
        }


        //Get all pages from last time we polled otherwise just get max pages.
        //The trick here is we have to keep fetching until we see lastPollId in the list then stop and remove any shit smaller than that.
        public async Task<List<Post>> GetImagesSinceLastPoll(int? lastPollId)
        {
            List<Post> results = new();
            while (true)
            {
                var posts = (await ScrapeImagesAfterId(lastPollId)).Posts; //Gets posts in oldest to newest order.

                if (posts.Count == 0)
                {
                    //No posts, bail right away.
                    break;
                }

                results.AddRange(posts);

                lastPollId = posts[0].Id; //Get the next newest batch, keep going until there is no newer posts.
            }

            return results; //done.
        }

        private IFlurlRequest Request()
        {
            IFlurlRequest request = BaseUrl.WithHeader("User-Agent", Config.UserAgent);
            if (!string.IsNullOrEmpty(_config.Username) && !string.IsNullOrEmpty(_config.ApiKey))
            {
                request = request.WithBasicAuth(_config.Username, _config.ApiKey);
            }

            return request;
        }
    }
}