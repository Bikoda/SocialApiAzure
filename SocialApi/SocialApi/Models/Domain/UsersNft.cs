namespace SocialApi.Models.Domain
{
    public class UsersNft
    {
        public int Id { get; set; }

        // Foreign key referencing the Users table
        public int UserId { get; set; }
        public Users User { get; set; } // Navigation property for User

        // Foreign key for Record, without a navigation property
        public int RecordId { get; set; }
    }
}
