using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class Records
    {

        [Key]
        public int RecordId { get; set; }
        public required string Path { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public bool IsNsfw { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
