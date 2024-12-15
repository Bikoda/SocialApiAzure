namespace SocialApi.Models.DTO
{
    public class OpenBidsDto
    {
        public long OpenBidId { get; set; }
        public long BidId { get; set; }
        public long NftId { get; set; }
        public long? HighestBidder { get; set; }
        public string Amount { get; set; }
        public DateTime CurrentTime { get; set; }
    }
}