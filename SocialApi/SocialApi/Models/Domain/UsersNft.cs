using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SocialApi.Models.Domain
{
    public class UsersNft
    {
        [Key]
        public long UserRecordId { get; set; }

        public long UserId { get; set; }

        public long RecordId { get; set; }

        public DateTime CreatedOn { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public Records Record { get; set; }
    }
}
