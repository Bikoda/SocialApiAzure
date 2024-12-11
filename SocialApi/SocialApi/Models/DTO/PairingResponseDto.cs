namespace SocialApi.Models.DTO
{
    public class PairingResponseDto
    {
        public string Message { get; set; }
        public long UserNftId { get; set; }

        public PairingResponseDto(string message, long userNftId)
        {
            Message = message;
            UserNftId = userNftId;
        }
    }
}
