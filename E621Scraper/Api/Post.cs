using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace E621Scraper.Api
{
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

        public List<string> AllTags => Tags.Values.SelectMany(x => x).ToList();

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
}