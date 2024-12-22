namespace SocialApi.Models.DTO
{
    public class PaginatedNftTagsResponse
    {
        public List<NftTagsDto> NftTags { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }

        public PaginatedNftTagsResponse(List<NftTagsDto> nftTags, int from, int to, int total, int totalPages)
        {
            NftTags = nftTags;
            From = from;
            To = to;
            Total = total;
            TotalPages = totalPages;
        }

        // Parameterless constructor for serialization
        public PaginatedNftTagsResponse() { }
    }
}
