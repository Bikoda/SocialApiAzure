using SocialApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

public class Nfts
{
    [Key]
    public long NftId { get; set; }

    [Required]
    public string NftAddress { get; set; }

    public long Views { get; set; } = 0;
    public long Likes { get; set; } = 0;

    public bool IsNsfw { get; set; }

    public DateTime CreatedOn { get; set; }

    public ICollection<NftTags> NftTags { get; set; } // This represents the relationship to Tags now

    public Nfts(string nftAddress, long views = 0, long likes = 0, bool isNsfw = false, DateTime? createdOn = null)
    {
        NftAddress = nftAddress ?? throw new ArgumentNullException(nameof(nftAddress));
        Views = views;
        Likes = likes;
        IsNsfw = isNsfw;
        CreatedOn = createdOn ?? DateTime.UtcNow;
    }

    public Nfts() { }
}
