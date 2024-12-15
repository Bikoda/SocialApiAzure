namespace SocialApi.Models.DTO
{
    public class AddBidHistoryDto
    {
        public long NftId { get; set; } // ID of the NFT being bid on

        public long UserId { get; set; } // ID of the user placing the bid

        public string Amount { get; set; } // The bid amount
    }
}
