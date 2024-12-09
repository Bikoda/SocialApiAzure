namespace SocialApi.Models.DTO
{
    public class AddUsersRequestDto
    {
        private DateTime _created = DateTime.UtcNow;

        public string? Nickname { get; set; }
        public string Email { get; set; }
        public string UserAddress { get; set; }
        public DateTime CreatedOn
        {
            get { return _created; }

        }

    }
}
