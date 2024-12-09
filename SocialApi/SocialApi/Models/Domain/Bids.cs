using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Bids
    {
        [Key]
        public long BidId { get; set; }
        public long Token { get; set; }
        public long StartAmmount { get; set; }
        public long HighestBidder { get; set; }
        public long BidOwner { get; set; }
        public bool IsEnded { get; set; }
        public DateTime AuctionEnds { get; set; }
        public DateTime AuctionStarts { get; set; }
    }
}
