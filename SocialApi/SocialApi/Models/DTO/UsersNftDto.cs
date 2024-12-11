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

        public string NftAddress { get; set; }  // Add NftAddress property here

        // Constructor for initialization
        public UsersNftDto(long userNftId, long userId, long nftId, DateTime createdOn, string nftAddress)
        {
            UserNftId = userNftId;
            UserId = userId;
            NftId = nftId;
            CreatedOn = createdOn;
            NftAddress = nftAddress;  // Initialize NftAddress
        }

        // Parameterless constructor (required for deserialization)
        public UsersNftDto() { }
    }
}
