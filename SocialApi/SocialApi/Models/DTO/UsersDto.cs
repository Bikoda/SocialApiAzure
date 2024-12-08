using SocialApi.Models.Domain;
using System.Numerics;

namespace SocialApi.Models.DTO
{
    public class UsersDto
    {
        private long _userId;

        public long UserId
        {
            get => _userId;
            set => _userId = value;
        }

        public required string Nickname { get; set; } // Ensures Nickname is always set.
        public required string Email { get; set; }    // Ensures Email is always set.
        public required string Address { get; set; }  // Ensures Address is always set.
        public DateTime CreatedOn { get; set; }

        // Constructor for initialization
        public UsersDto(string userId, string nickname, string email, string address, DateTime createdOn)
        {
            if (!long.TryParse(userId, out _userId))
                throw new ArgumentException($"Invalid long value for {nameof(userId)}: {userId}");

            Nickname = nickname;
            Email = email;
            Address = address;
            CreatedOn = createdOn;
        }

        // Parameterless constructor (required for deserialization)
        public UsersDto() { }
    }

}
