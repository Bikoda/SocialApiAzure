using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SocialApi.Models.Domain
{
    public class UsersNft
    {
        [Key]
        public long UserNftId { get; set; }

        public long UserId { get; set; }

        public long NftId { get; set; }

        public DateTime CreatedOn { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public Nfts Nft { get; set; }
    }
}
