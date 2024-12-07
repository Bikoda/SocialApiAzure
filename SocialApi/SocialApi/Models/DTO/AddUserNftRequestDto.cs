namespace SocialApi.Models.DTO
{
    public class AddUserNftRequestDto
    {
        private DateTime _created = DateTime.UtcNow;
        public string Address { get; set; }
        public string Path { get; set; }

        public int? RecordId { get; set; }

        public DateTime CreatedOn
        {
            get { return _created; }

        }
    }
}
