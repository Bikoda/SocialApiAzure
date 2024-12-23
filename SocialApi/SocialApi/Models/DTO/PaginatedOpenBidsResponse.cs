using SocialApi.Models.DTO;

public class PaginatedOpenBidsResponse
{
    public List<OpenBidsDto> OpenBids { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}
