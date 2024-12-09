using System.Numerics;

namespace SocialApi.Models.DTO
{
    public class AddUserNftRequestDto
    {

        private DateTime _created = DateTime.UtcNow;

        public string UserAddress { get; set; }
        public string NftAddress { get; set; }
        public string? NftId { get; set; }

        public DateTime CreatedOn
        {
            get { return _created; }

        }
    }
}
