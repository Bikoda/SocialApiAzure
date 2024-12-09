using SocialApi.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Net;

namespace SocialApi.Models.DTO
{
    public class UsersNftDto
    {
        [Key]
        public long UserNftId { get; set; }

        // Foreign key referencing the Users table
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public Users User { get; set; } // Navigation property for User

        // Foreign key for Nft
        [ForeignKey("Nft")]
        public long NftId { get; set; }

        public DateTime CreatedOn { get; set; }

        // Constructor for initialization
        public UsersNftDto(string userNftId, string userId, string nftId, DateTime createdOn)
        {
            if (!long.TryParse(userNftId, out var parsedUserNftId))
                throw new ArgumentException($"Invalid long value for {nameof(userNftId)}: {userNftId}");
            UserNftId = parsedUserNftId;

            if (!long.TryParse(userId, out var parsedUserId))
                throw new ArgumentException($"Invalid long value for {nameof(userId)}: {userId}");
            UserId = parsedUserId;

            if (!long.TryParse(nftId, out var parsedNftId))
                throw new ArgumentException($"Invalid long value for {nameof(nftId)}: {nftId}");
            NftId = parsedNftId;

            CreatedOn = createdOn;
        }

        // Parameterless constructor (required for deserialization)
        public UsersNftDto() { }
    }
}
