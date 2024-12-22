using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.DTO
{
    public class TagsDto
    {

        private long _tagId;

        public long TagId
        {
            get => _tagId;
            set => _tagId = value;
        }
        public string Name { get; set; }
        public string? Description { get; set; }

        public string? Image { get; set; }

        public string? External_url { get; set; }

        public string? Attributes { get; set; }
    }
}
