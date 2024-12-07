using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }

        public ICollection<UsersNft> UserNfts { get; set; }
    }
}
