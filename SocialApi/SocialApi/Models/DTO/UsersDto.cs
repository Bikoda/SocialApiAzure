namespace SocialApi.Models.DTO
{
    public class UsersDto
    {
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
