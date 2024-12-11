using System.ComponentModel.DataAnnotations;

namespace SocialApi.Models.DTO
{
    public class AddUsersRequestDto
    {
        // Nullable Nickname (can be null)
        public string? Nickname { get; set; }

        // Required properties with validation
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "User address is required")]
        public string UserAddress { get; set; }

        // Read-only CreatedOn property
        public DateTime CreatedOn { get; } = DateTime.UtcNow; // Default to UTC now

        // Constructor to initialize required fields
        public AddUsersRequestDto(string email, string userAddress, string? nickname = null)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            UserAddress = userAddress ?? throw new ArgumentNullException(nameof(userAddress));
            Nickname = nickname;
        }

        // Parameterless constructor (required for deserialization)
        public AddUsersRequestDto() { }
    }
}