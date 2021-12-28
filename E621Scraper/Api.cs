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
        private const string BaseUrl = "https://e621.net/";
        private readonly string _apiKey;
        private readonly string _username;

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

        private Task<PostsCollection> ScrapeImagesBeforeId(int? id)
        {
            if (id == null)
            {
                return ScrapeImages();
            }

            return Request().AppendPathSegment("posts.json")
                            .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = $"b{id}"})
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
                            .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = $"a{lastId}"})
                            .GetJsonAsync<PostsCollection>();
        }

        public Task<PostsCollection> ScrapeImages()
        {
            return Request().AppendPathSegment("posts.json")
                            .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = "2"})
                            .GetJsonAsync<PostsCollection>();
        }


        //Get all pages from last time we polled otherwise just get max pages.
        //The trick here is we have to keep fetching until we see lastPollId in the list then stop and remove any shit smaller than that.
        public async Task<List<Post>> GetImagesSinceLastPoll(int? lastPollId)
        {
            List<Post> results = new();
            while (
                true) // I think this logic might still be a bit fucked, lets fuck off this nullable lastpollid shit and just do an upfront call seperate
            {
                var posts = (await ScrapeImagesAfterId(lastPollId)).Posts; //Gets posts in oldest to newest order.

                if (posts.Count == 0)
                {
                    //No posts, bail right away.
                    break;
                }

                await Task.Delay(1000);

                results.AddRange(posts);

                lastPollId = posts[0].Id; //Get the next newest batch, keep going until there is no newer posts.
            }

            return results; //done.
        }

        private IFlurlRequest Request()
        {
            return BaseUrl.WithBasicAuth(_username, _apiKey)
                          .WithHeader("User-Agent", Config.UserAgent);
        }
    }
}