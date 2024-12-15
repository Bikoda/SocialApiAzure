using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApi.Models.Domain
{
    public class OpenBids
    {
        [Key]
        public long OpenBidId { get; set; }

        [ForeignKey("Bids")]
        public long BidId { get; set; }
        public Bids Bid { get; set; } // Navigation property

        [ForeignKey("Nfts")]
        public long NftId { get; set; }
        public Nfts Nft { get; set; } // Navigation property

        [ForeignKey("Users")]
        public long? HighestBidder { get; set; } = 0;   
        public Users User { get; set; } // Navigation property

        [Required]
        public string Amount { get; set; } = "0";

        public DateTime CurrentTime { get; set; }
    }
}