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
        [JsonProperty("posts")]
        public List<Post>? Posts { get; set; }
    }
    
    public class Post
    {
        [JsonIgnore] private Dictionary<string, List<string>> _tags = new();

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created_at")]
        public string? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string? UpdatedAt { get; set; }

        [JsonProperty("file")]
        public FileArray? File { get; set; }

        [JsonProperty("preview")]
        public PreviewArray? Preview { get; set; }

        [JsonProperty("sample")]
        public SampleArray? Sample { get; set; }

        [JsonProperty("score")]
        public ScoreArray? Score { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, List<string>> Tags
        {
            get
            {
                if (!_tags.ContainsKey("all"))
                {
                    _tags.Add("all", new List<string>());
                }

                List<string> allTags = _tags["all"];
                foreach (var tag in _tags)
                {
                    allTags.AddRange(tag.Value);
                }

                _tags["all"] = allTags.Distinct().ToList();

                return _tags;
            }
            set => _tags = value;
        }

        [JsonProperty("locked_tags")]
        public List<string>? LockedTags { get; set; }

        [JsonProperty("change_seq")]
        public string? ChangeSeq { get; set; }

        [JsonProperty("flags")]
        public FlagsArray? Flags { get; set; }

        [JsonProperty("rating")]
        public string? Rating { get; set; }

        [JsonProperty("fav_count")]
        public string? FavCount { get; set; }

        [JsonProperty("sources")]
        public List<string>? Sources { get; set; }

        [JsonProperty("pools")]
        public List<string>? Pools { get; set; }

        [JsonProperty("relationships")]
        public RelationshipArray? Relationships { get; set; }

        [JsonProperty("approver_id")]
        public string? ApproverId { get; set; }

        [JsonProperty("uploader_id")]
        public string? UploaderId { get; set; }

        [JsonProperty("descriptions")]
        public string? Descriptions { get; set; }

        [JsonProperty("comment_count")]
        public string? CommentCount { get; set; }

        [JsonProperty("is_favorited")]
        public bool? IsFavorited { get; set; }

        public class FileArray
        {
            [JsonProperty("width")]
            public string? Width { get; set; }

            [JsonProperty("height")]
            public string? Heigth { get; set; }

            [JsonProperty("ext")]
            public string? Ext { get; set; }

            [JsonProperty("size")]
            public string? Size { get; set; }

            [JsonProperty("md5")]
            public string? Md5 { get; set; }

            [JsonProperty("url")]
            public string? Url { get; set; }
        }

        public class PreviewArray
        {
            [JsonProperty("width")]
            public string? Width { get; set; }

            [JsonProperty("height")]
            public string? Height { get; set; }

            [JsonProperty("url")]
            public string? Url { get; set; }
        }

        public class SampleArray
        {
            [JsonProperty("has")]
            public bool? Has { get; set; }

            [JsonProperty("width")]
            public string? Width { get; set; }

            [JsonProperty("height")]
            public string? Height { get; set; }

            [JsonProperty("url")]
            public string? Url { get; set; }
        }

        public class ScoreArray
        {
            [JsonProperty("up")]
            public string? Up { get; set; }

            [JsonProperty("down")]
            public string? Down { get; set; }

            [JsonProperty("total")]
            public string? Total { get; set; }
        }

        public class FlagsArray
        {
            [JsonProperty("Pending")]
            public bool? Pending { get; set; }

            [JsonProperty("flagged")]
            public bool? Flagged { get; set; }

            [JsonProperty("note_locked")]
            public bool? NoteLocked { get; set; }

            [JsonProperty("status_locked")]
            public bool? StatusLocked { get; set; }

            [JsonProperty("rating_locked")]
            public bool? RatingLocked { get; set; }

            [JsonProperty("deleted")]
            public bool? Deleted { get; set; }
        }

        public class RelationshipArray
        {
            [JsonProperty("parent_id")]
            public string? ParentID { get; set; }

            [JsonProperty("has_children")]
            public bool? HasChildren { get; set; }

            [JsonProperty("has_active_children")]
            public bool? HasActiveChildren { get; set; }

            [JsonProperty("children")]
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
                    NamingStrategy = new CamelCaseNamingStrategy()
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