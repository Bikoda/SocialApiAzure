namespace SocialApi.Models.DTO
{
    public class AddUsersRequestDto
    {
        private DateTime _created = DateTime.Now;

        public string? Nickname { get; set; }
        public int Messages { get; set; }
        public string Address { get; set; }
        public DateTime CreatedOn
        {
            get { return _created; }

        }
    }
}
