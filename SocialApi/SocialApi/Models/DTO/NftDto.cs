using System.Numerics;

namespace SocialApi.Models.DTO
{

    public class NftDto
    {
        private long _nftId;
        private long _views;
        private long _likes;

        public long NftId
        {
            get => _nftId;
            set => _nftId = value;
        }

        public required string NftAddress { get; set; } 

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
        public NftDto(string nftId, string nftAddress, string views, string likes, bool isNsfw, DateTime createdOn)
        {
            if (!long.TryParse(nftId, out _nftId))
                throw new ArgumentException($"Invalid long value for {nameof(nftId)}: {nftId}");

            NftAddress = nftAddress;

            if (!long.TryParse(views, out _views))
                throw new ArgumentException($"Invalid long value for {nameof(views)}: {views}");

            if (!long.TryParse(likes, out _likes))
                throw new ArgumentException($"Invalid long value for {nameof(likes)}: {likes}");

            IsNsfw = isNsfw;
            CreatedOn = createdOn;
        }

        // Parameterless constructor (required for deserialization)
        public NftDto() { }
    }
}
