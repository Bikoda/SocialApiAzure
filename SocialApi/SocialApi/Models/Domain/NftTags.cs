namespace SocialApi.Models.Domain
{
    public class NftTags
    {
        public long NftId { get; set; }
        public long TagId { get; set; }
        public DateTime CreatedOn { get; set; }

        public Nfts Nft { get; set; }
        public Tags Tag { get; set; }
    }
}
