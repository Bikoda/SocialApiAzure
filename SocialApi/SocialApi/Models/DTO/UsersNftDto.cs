using SocialApi.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.DTO
{
    public class UsersNftDto
    {
        [Key]
        public int Id { get; set; }

        // Foreign key referencing the Users table
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public Users User { get; set; } // Navigation property for User

        // Foreign key for Record
        [ForeignKey("Record")]
        public int RecordId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
