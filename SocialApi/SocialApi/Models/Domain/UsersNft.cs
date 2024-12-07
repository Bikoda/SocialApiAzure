using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.Domain
{
    public class UsersNft
    {
        [Key]
        public int UserRecordId { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; } // Navigation property for User

        public int RecordId { get; set; }
        public Records Record { get; set; } // Navigation property for Record

        public DateTime CreatedOn { get; set; }
    }
}
