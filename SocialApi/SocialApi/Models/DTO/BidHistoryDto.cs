namespace SocialApi.Models.DTO
{
    public class BidHistoryDto
    {
        public long BidHistoryId { get; set; } // ID of the bid history entry

        public long NftId { get; set; } // ID of the NFT being bid on

        public long UserId { get; set; } // ID of the user who placed the bid

        public string Amount { get; set; } // The bid amount

        public DateTime CurrentTime { get; set; } // Timestamp for the bid
    }
}