using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Users
    {
        [Key]
        public long UserId { get; set; }

        // Required fields with validation for non-null
        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserAddress { get; set; }

        public DateTime CreatedOn { get; set; }

        // Navigation property for related NFTs - Initialize to avoid null reference issues
        public ICollection<UsersNft> UserNfts { get; set; } = new List<UsersNft>(); // Initialize as an empty collection

        // Constructor for initialization
        public Users(long userId, string nickname, string email, string userAddress, DateTime createdOn, ICollection<UsersNft> userNfts)
        {
            UserId = userId;
            Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname)); // Ensure Nickname is not null
            Email = email ?? throw new ArgumentNullException(nameof(email)); // Ensure Email is not null
            UserAddress = userAddress ?? throw new ArgumentNullException(nameof(userAddress)); // Ensure UserAddress is not null
            CreatedOn = createdOn;
            UserNfts = userNfts ?? throw new ArgumentNullException(nameof(userNfts)); // Ensure UserNfts is not null
        }

        // Parameterless constructor (required for deserialization)
        public Users() { }
    }
}