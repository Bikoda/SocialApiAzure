using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class UsersNft
    {
        [Key]
        public long UserNftId { get; set; }

        public long UserId { get; set; }

        public long NftId { get; set; }

        public DateTime CreatedOn { get; set; }

        // Navigation properties - Mark as nullable if they can be null, or use constructor for initialization
        public Users? User { get; set; }  // Nullable if the User can be null
        public Nfts? Nft { get; set; }  // Nullable if the Nft can be null

        // Constructor to initialize non-nullable properties if needed
        public UsersNft(long userNftId, long userId, long nftId, DateTime createdOn, Users user, Nfts nft)
        {
            UserNftId = userNftId;
            UserId = userId;
            NftId = nftId;
            CreatedOn = createdOn;
            User = user ?? throw new ArgumentNullException(nameof(user)); // Ensure User is not null
            Nft = nft ?? throw new ArgumentNullException(nameof(nft)); // Ensure Nft is not null
        }

        // Parameterless constructor (required for deserialization)
        public UsersNft() { }
    }
}