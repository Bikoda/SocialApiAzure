namespace SocialApi.Models.DTO
{
    public class AddRecordsRequestDto
    {
        private DateTime _created = DateTime.Now;

        public required string Path { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public bool IsNsfw { get; set; }
        public DateTime CreatedOn
        {
            get { return _created; }

        }
    }
}
