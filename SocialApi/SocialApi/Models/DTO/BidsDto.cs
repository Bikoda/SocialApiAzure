namespace SocialApi.Models.DTO
{
    public class BidsDto
    {
        public long BidId { get; set; }
        public long NftId { get; set; }
        public string StartAmount { get; set; }
        public DateTime EndTime { get; set; }
        public long? HighestBidder { get; set; }
        public string Amount { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CurrentTime { get; set; }
    }
}