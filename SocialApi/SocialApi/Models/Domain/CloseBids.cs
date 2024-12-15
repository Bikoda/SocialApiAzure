using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApi.Models.Domain
{
    public class CloseBids
    {
        [Key]
        public long CloseBidId { get; set; }

        [ForeignKey("Bids")]
        public long BidId { get; set; }
        public Bids Bid { get; set; } // Navigation property

        [ForeignKey("Nfts")]
        public long NftId { get; set; }
        public Nfts Nft { get; set; } // Navigation property

        [ForeignKey("Users")]
        public long? HighestBidder { get; set; }
        public Users User { get; set; } // Navigation property

        [Required]
        public string Amount { get; set; }

        public DateTime CloseTime { get; set; }
    }
}