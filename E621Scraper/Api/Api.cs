using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E621Scraper.Configs;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// TODO: I think the cancellation checks should be put at the start, not strewn throughout.

namespace E621Scraper.Api
{
    public class Api
    {
        private const string BaseUrl = "https://e621.net/";
        private readonly ApiConfig _config;
        private readonly ILogger<Api> _logger;

        public Api(ApiConfig config, ILogger<Api> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

        private async Task<PostsCollection> ScrapeImagesBeforeId(int? id, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (id == null)
                {
                    return await ScrapeImages(cancellationToken);
                }

                await Task.Delay(1000, cancellationToken);

                return await Request().AppendPathSegment("posts.json")
                                      .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = $"b{id}"})
                                      .GetJsonAsync<PostsCollection>(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("ScrapeImagesBeforeId Cancelled");
                throw;
            }
        }

        // TODO: add a global ratelimit timer so that it is impossible to get limited
        private async Task<PostsCollection> ScrapeImagesAfterId(int? lastId,
                                                                CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (lastId == null)
                {
                    return await ScrapeImages(cancellationToken);
                }

                await Task.Delay(1000, cancellationToken);

                return await Request().AppendPathSegment("posts.json")
                                      .SetQueryParams(new {limit = Config.MaxPostsPerRequest, page = $"a{lastId}"})
                                      .GetJsonAsync<PostsCollection>(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("ScrapeImagesAfterId Cancelled");
                throw;
            }
        }

        private async Task<PostsCollection> ScrapeImages(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(1000, cancellationToken);

                return await Request().AppendPathSegment("posts.json")
                                      .SetQueryParams(new {limit = 50, page = "1"})
                                      .GetJsonAsync<PostsCollection>(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("ScrapeImages Cancelled");
                throw;
            }
        }

        //Get all pages from last time we polled otherwise just get max pages.
        //The trick here is we have to keep fetching until we see lastPollId in the list then stop and remove any shit smaller than that.
        // TODO: Improve cancellation handling.
        public async Task<List<Post>> GetImagesSinceLastPoll(int? lastPollId,
                                                             CancellationToken cancellationToken = default)
        {
            try
            {
                List<Post> results = new();
                while (true)
                {
                    var posts = (await ScrapeImagesAfterId(lastPollId, cancellationToken))
                        .Posts; //Gets posts in oldest to newest order.

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
            catch (TaskCanceledException)
            {
                return new List<Post>();
            }
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