using System.Numerics;

namespace SocialApi.Models.DTO
{

    public class RecordsDto
    {
        private long _recordId;
        private long _views;
        private long _likes;

        public long RecordId
        {
            get => _recordId;
            set => _recordId = value;
        }

        public required string Path { get; set; } 

        public long Views
        {
            get => _views;
            set => _views = value;
        }

        public long Likes
        {
            get => _likes;
            set => _likes = value;
        }

        public bool IsNsfw { get; set; }
        public DateTime CreatedOn { get; set; }

        // Constructor for initialization
        public RecordsDto(string recordId, string path, string views, string likes, bool isNsfw, DateTime createdOn)
        {
            if (!long.TryParse(recordId, out _recordId))
                throw new ArgumentException($"Invalid long value for {nameof(recordId)}: {recordId}");

            Path = path;

            if (!long.TryParse(views, out _views))
                throw new ArgumentException($"Invalid long value for {nameof(views)}: {views}");

            if (!long.TryParse(likes, out _likes))
                throw new ArgumentException($"Invalid long value for {nameof(likes)}: {likes}");

            IsNsfw = isNsfw;
            CreatedOn = createdOn;
        }

        // Parameterless constructor (required for deserialization)
        public RecordsDto() { }
    }
}
