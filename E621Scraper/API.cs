using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace E621Scraper
{
    public class PostsCollection
    {
        public List<Post> Posts { get; set; } = new();
    }

    public class Post
    {
        [JsonIgnore] private Dictionary<string, List<string>> _tags = new();

        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public FileArray File { get; set; } = new();

        public PreviewArray? Preview { get; set; }

        public SampleArray? Sample { get; set; }

        public ScoreArray? Score { get; set; }

        public List<string> AllTags
        {
            get
            {
                return Tags.Values.SelectMany(x => x).ToList();
            }
        }

        public Dictionary<string, List<string>> Tags { get; set; } = new();

        public List<string>? LockedTags { get; set; }

        public int ChangeSeq { get; set; }

        public FlagsArray? Flags { get; set; }

        public string Rating { get; set; } = default!;

        public int FavCount { get; set; }

        public List<string>? Sources { get; set; }

        public List<string>? Pools { get; set; }

        public RelationshipArray? Relationships { get; set; }

        public int? ApproverId { get; set; }

        public int UploaderId { get; set; }

        public string Descriptions { get; set; } = default!;

        public int CommentCount { get; set; }

        public bool IsFavorited { get; set; }

        public class FileArray
        {
            public int Width { get; set; }

            public int Heigth { get; set; }

            public string Ext { get; set; } = default!;

            public int Size { get; set; }

            public string Md5 { get; set; } = default!;

            public string Url { get; set; } = default!;
        }

        public class PreviewArray
        {
            public int Width { get; set; }

            public int Height { get; set; }

            public string Url { get; set; } = default!;
        }

        public class SampleArray
        {
            public bool Has { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public string Url { get; set; } = default!;
        }

        public class ScoreArray
        {
            public int Up { get; set; }

            public int Down { get; set; }

            public int Total { get; set; }
        }

        public class FlagsArray
        {
            public bool Pending { get; set; }

            public bool Flagged { get; set; }

            public bool NoteLocked { get; set; }

            public bool StatusLocked { get; set; }

            public bool RatingLocked { get; set; }

            public bool Deleted { get; set; }
        }

        public class RelationshipArray
        {
            public int? ParentId { get; set; }

            public bool HasChildren { get; set; }

            public bool HasActiveChildren { get; set; }

            public List<string>? Children { get; set; }
        }
    }

    public class Api
    {
        private const string UserAgent = "TeleBotTest/0.1 (by Zylinx on e621)";
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

        // TODO: Add handling of pages
        public async Task<PostsCollection> ScrapeImages(string lastId)
        {
            var result = await BaseUrl.WithBasicAuth(_username, _apiKey)
                                      .WithHeader("User-Agent", UserAgent)
                                      .AppendPathSegment("posts.json")
                                      .SetQueryParam("limit", 370)
                                      .SetQueryParam("page", $"a{lastId}")
                                      .GetJsonAsync<PostsCollection>();

            return result;
        }
    }
}