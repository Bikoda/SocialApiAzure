using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApi.Data
{
    public interface IWebSocialDbContext
    {
        DbSet<Bids> Bids { get; }
        DbSet<OpenBids> OpenBids { get; }
        DbSet<CloseBids> CloseBids { get; }
        DbSet<BidHistory> BidHistory { get; }
        DbSet<Nfts> Nfts { get; }
        DbSet<Users> Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}