using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SocialApi.Models.Domain
{
    public class Records
    {

        [Key]
        public long RecordId { get; set; }
        public string Path { get; set; }
        public long Views { get; set; }
        public long Likes { get; set; }
        public bool IsNsfw { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
