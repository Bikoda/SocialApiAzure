using System.Numerics;

namespace SocialApi.Models.DTO
{
    public class AddNftRequestDto
    {
        private DateTime _created = DateTime.Now;

        public required string NftAddress { get; set; }
        public string Views { get; set; }


        public string Likes { get; set; }
        public bool IsNsfw { get; set; }
        public DateTime CreatedOn
        {
            get { return _created; }

        }
    }
}
