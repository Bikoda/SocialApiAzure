using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Users
    {
        [Key]
        public long UserId { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string UserAddress { get; set; }
        public DateTime CreatedOn { get; set; }

        // Navigation property for related NFTs
        public ICollection<UsersNft> UserNfts { get; set; }
    }
}
