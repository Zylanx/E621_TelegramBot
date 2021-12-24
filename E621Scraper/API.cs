using System.Collections.Generic;

namespace E621Scraper
{
    public class Post
    {
        public string ID { get; }
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

    public class API
    {
    }
}