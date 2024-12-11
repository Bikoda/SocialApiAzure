using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Nfts // Keeping the name as Nfts
    {
        [Key]
        public long NftId { get; set; }

        // Mark as required since the address is crucial to an NFT
        [Required]
        public string NftAddress { get; set; }

        // Views and Likes initialized to 0 if not passed
        public long Views { get; set; } = 0;
        public long Likes { get; set; } = 0;

        public bool IsNsfw { get; set; }

        public DateTime CreatedOn { get; set; }

        // Constructor for initialization
        public Nfts(string nftAddress, long views = 0, long likes = 0, bool isNsfw = false, DateTime? createdOn = null)
        {
            NftAddress = nftAddress ?? throw new ArgumentNullException(nameof(nftAddress));
            Views = views;
            Likes = likes;
            IsNsfw = isNsfw;
            CreatedOn = createdOn ?? DateTime.UtcNow;  // Default to current UTC time if not provided
        }

        // Parameterless constructor (required for deserialization)
        public Nfts() { }
    }
}