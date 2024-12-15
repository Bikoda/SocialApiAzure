namespace SocialApi.Models.DTO
{
    public class CloseBidsDto
    {
        public long CloseBidId { get; set; }
        public long BidId { get; set; }
        public long NftId { get; set; }
        public long? HighestBidder { get; set; }
        public string Amount { get; set; }
        public DateTime CloseTime { get; set; }
    }
}