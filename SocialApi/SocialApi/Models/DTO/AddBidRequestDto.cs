namespace SocialApi.Models.DTO
{
    public class AddBidRequestDto
    {
        public long NftId { get; set; }
        public string StartAmount { get; set; }
        public DateTime EndTime { get; set; }
    }
}