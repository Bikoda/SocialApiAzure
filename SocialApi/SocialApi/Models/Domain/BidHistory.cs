using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApi.Models.Domain
{
    public class BidHistory
    {
        [Key]
        public long BidHistoryId { get; set; } // Primary key

        [Required]
        [ForeignKey("Nfts")]
        public long NftId { get; set; } // Foreign key to the Nfts table
        public Nfts Nft { get; set; } // Navigation property

        [Required]
        [ForeignKey("Users")]
        public long UserId { get; set; } // Foreign key to the Users table
        public Users User { get; set; } // Navigation property

        [Required]
        public string Amount { get; set; } // Bid amount (non-nullable)

        [Required]
        public DateTime CurrentTime { get; set; } // Timestamp for when the bid was placed
    }
}