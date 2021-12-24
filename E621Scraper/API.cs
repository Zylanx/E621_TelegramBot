using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace E621Scraper
{
    public class Post
    {
        public string Id { get; }
        public string CreatedAt { get; }
        public string UpdatedAt { get; }
        public FileArray File { get; }
        public PreviewArray Preview { get; }
        public SampleArray Sample { get; }
        public ScoreArray Score { get; }
        public Dictionary<string, List<string>> Tags { get; }
        public List<string> LockedTags { get; }
        public string ChangeSeq { get; }
        public FlagsArray Flags { get; }
        public string Rating { get; }
        public string FavCount { get; }
        public string Sources { get; }
        public string Pools { get; }
        public RelationshipArray Relationships { get; }
        public string ApproverId { get; }
        public string UploaderId { get; }
        public string Descriptions { get; }
        public string CommentCount { get; }
        public bool IsFavorited { get; }

        public class FileArray
        {
            public string Width { get; }
            public string Heigth { get; }
            public string Ext { get; }
            public string Size { get; }
            public string Md5 { get; }
            public string Url { get; }
        }

        public class PreviewArray
        {
            public string Width { get; }
            public string Height { get; }
            public string Url { get; }
        }

        public class SampleArray
        {
            public bool Has { get; }
            public string Width { get; }
            public string Height { get; }
            public string Url { get; }
        }

        public class ScoreArray
        {
            public string Up { get; }
            public string Down { get; }
            public string Total { get; }
        }

        public class FlagsArray
        {
            public bool Pending { get; }
            public bool Flagged { get; }
            public bool NoteLocked { get; }
            public bool StatusLocked { get; }
            public bool RatingLocked { get; }
            public bool Deleted { get; }
        }

        public class RelationshipArray
        {
            public string ParentID { get; }
            public bool HasChildren { get; }
            public bool HasActiveChildren { get; }
            public List<string> Children { get; }
        }
    }

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

        // TODO: Add handling of pages
        public async Task<List<Post>> ScrapeImages(string lastId)
        {
            return await BaseUrl.WithBasicAuth(_username, _apiKey)
                                .AppendPathSegment("posts")
                                .SetQueryParam("limit", 320)
                                .SetQueryParam("page", $"a{lastId}")
                                .GetJsonAsync<List<Post>>();
        }
    }
}