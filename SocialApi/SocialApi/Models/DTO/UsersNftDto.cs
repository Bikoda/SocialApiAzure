using SocialApi.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Net;

namespace SocialApi.Models.DTO
{
    public class UsersNftDto
    {
        [Key]
        public long UserRecordId { get; set; }

        // Foreign key referencing the Users table
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public Users User { get; set; } // Navigation property for User

        // Foreign key for Record
        [ForeignKey("Record")]
        public long RecordId { get; set; }

        public DateTime CreatedOn { get; set; }

        // Constructor for initialization
        public UsersNftDto(string userRecordId, string userId, string recordId, DateTime createdOn)
        {
            if (!long.TryParse(userRecordId, out var parsedUserRecordId))
                throw new ArgumentException($"Invalid long value for {nameof(userRecordId)}: {userRecordId}");
            UserRecordId = parsedUserRecordId;

            if (!long.TryParse(userId, out var parsedUserId))
                throw new ArgumentException($"Invalid long value for {nameof(userId)}: {userId}");
            UserId = parsedUserId;

            if (!long.TryParse(recordId, out var parsedRecordId))
                throw new ArgumentException($"Invalid long value for {nameof(recordId)}: {recordId}");
            RecordId = parsedRecordId;

            CreatedOn = createdOn;
        }

        // Parameterless constructor (required for deserialization)
        public UsersNftDto() { }
    }
}
