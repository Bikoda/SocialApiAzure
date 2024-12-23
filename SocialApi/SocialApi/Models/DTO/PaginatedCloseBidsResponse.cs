namespace SocialApi.Models.DTO
{
    public class PaginatedCloseBidsResponse
    {
        public List<CloseBidsDto> CloseBids { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}
