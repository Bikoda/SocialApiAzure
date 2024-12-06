namespace SocialApi.Models.DTO
{
    public class UsersDto
    {
        public int Id { get; set; }
        public string? Nickname { get; set; }
        public int Messages { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
