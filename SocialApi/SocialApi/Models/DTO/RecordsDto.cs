namespace SocialApi.Models.DTO
{
    public class RecordsDto
    {

        public int Id { get; set; }
        public required string Path { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public bool IsNsfw { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
