using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApi.Models.Domain
{
    public class Bids
    {
        [Key]
        public long BidId { get; set; }

        [ForeignKey("Nfts")]
        public long NftId { get; set; }
        public Nfts Nft { get; set; } // Navigation property

        [Required]
        public string StartAmount { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public long? HighestBidder { get; set; } = 0; // FK to Users table
        public Users User { get; set; } // Navigation property

        public string Amount { get; set; } = "0";

        public bool IsOpen { get; set; } = true; // To indicate whether the bid is active

        public DateTime CurrentTime { get; set; }
    }
}