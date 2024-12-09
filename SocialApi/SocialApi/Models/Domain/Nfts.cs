using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SocialApi.Models.Domain
{
    public class Nfts //cambiar a nft
    {

        [Key]
        public long NftId { get; set; }
        public string NftAddress { get; set; }
        public long Views { get; set; }
        public long Likes { get; set; }
        public bool IsNsfw { get; set; }

        //public string TokenUri { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
