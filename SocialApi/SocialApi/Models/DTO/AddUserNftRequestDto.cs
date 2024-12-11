using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.DTO
{
    public class AddUserNftRequestDto
    {
        // Required properties with validation
        [Required(ErrorMessage = "User Address is required")]
        public string UserAddress { get; set; }

        [Required(ErrorMessage = "NFT Address is required")]
        public string NftAddress { get; set; }

        public string? NftId { get; set; }

        // Read-only CreatedOn property (defaults to current UTC time)
        public DateTime CreatedOn { get; } = DateTime.UtcNow;

        // Parameterless constructor (required for deserialization)
        public AddUserNftRequestDto() { }

        // Constructor to initialize required fields
        public AddUserNftRequestDto(string userAddress, string nftAddress, string? nftId = null)
        {
            UserAddress = userAddress ?? throw new ArgumentNullException(nameof(userAddress));
            NftAddress = nftAddress ?? throw new ArgumentNullException(nameof(nftAddress));
            NftId = nftId;
        }
    }
}
