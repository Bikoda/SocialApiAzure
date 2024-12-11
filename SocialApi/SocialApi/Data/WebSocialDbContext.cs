using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApi.Data
{
    public class WebSocialDbContext : DbContext, IWebSocialDbContext
    {
        public WebSocialDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public virtual DbSet<Users> Users { get; set; } // Marked virtual for mocking
        public virtual DbSet<Nfts> Nfts { get; set; } // Marked virtual for mocking
        public virtual DbSet<UsersNft> UserNfts { get; set; } // Marked virtual for mocking

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure UsersNft primary key
            modelBuilder.Entity<UsersNft>()
                .HasKey(un => un.UserNftId);

            // Configure Users navigation property
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNfts)
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Records navigation property
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.Nft)
                .WithMany()
                .HasForeignKey(un => un.NftId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }

    public interface IWebSocialDbContext
    {
        DbSet<Users> Users { get; }
        DbSet<Nfts> Nfts { get; }
        DbSet<UsersNft> UserNfts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}