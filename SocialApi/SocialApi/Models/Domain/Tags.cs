using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Tags
    {
        [Key]
        public long TagId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public string? Image  { get; set; }

        public string? External_url { get; set; }

        public string? Attributes { get; set; }

        public ICollection<NftTags> NftTags { get; set; }


        // Navigation property to related Nfts
        public ICollection<Nfts> Nfts { get; set; }  // A tag can have many NFTs
    }
}
