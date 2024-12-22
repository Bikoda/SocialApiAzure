namespace SocialApi.Models.DTO
{
    public class NftTagsDto
    {
        public long NftId { get; set; }
        public long TagId { get; set; }
        public DateTime CreatedOn { get; set; }

        public NftTagsDto(long nftId, long tagId, DateTime createdOn)
        {
            NftId = nftId;
            TagId = tagId;
            CreatedOn = createdOn;
        }

        // Parameterless constructor for serialization
        public NftTagsDto() { }
    }
}
