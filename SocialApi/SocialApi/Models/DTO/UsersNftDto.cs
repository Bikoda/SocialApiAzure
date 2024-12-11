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

        // Required navigation property for User
        [Required]
        public Users User { get; set; } // Navigation property for User

        // Foreign key for Nft
        [ForeignKey("Nft")]
        public long NftId { get; set; }

        public DateTime CreatedOn { get; set; }

        // Required property for NftAddress
        [Required]
        public string NftAddress { get; set; }

        // Constructor for initialization
        public UsersNftDto(long userNftId, long userId, long nftId, DateTime createdOn, string nftAddress)
        {
            UserNftId = userNftId;
            UserId = userId;
            NftId = nftId;
            CreatedOn = createdOn;
            NftAddress = nftAddress ?? throw new ArgumentNullException(nameof(nftAddress), "NftAddress cannot be null.");  // Ensure NftAddress is not null
        }

        // Parameterless constructor (required for deserialization)
        public UsersNftDto() { }
    }
}