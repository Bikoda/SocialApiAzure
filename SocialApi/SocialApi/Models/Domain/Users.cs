using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string? Nickname { get; set; }
        public int Messages { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }

        public ICollection<UsersNft> UserNfts { get; set; }
    }
}
