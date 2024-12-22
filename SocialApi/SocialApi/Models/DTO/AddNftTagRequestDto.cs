namespace SocialApi.Models.DTO
{
    public class AddNftTagRequestDto
    {
        public long NftId { get; set; } // The ID of the NFT
        public List<long> TagIds { get; set; } // A list of Tag IDs to associate with the NFT

        // Read-only CreatedOn property
        public DateTime CreatedOn { get; } = DateTime.UtcNow; // Default to UTC now
    }
}
