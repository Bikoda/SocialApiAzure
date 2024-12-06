using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using System.Collections.Generic;

namespace SocialApi.Data
{
    public class WebSocialDbContext : DbContext
    {
        public WebSocialDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }


        public DbSet<Users> LogUser { get; set; } // Existing DbSet for Users
        public DbSet<UsersNft> UserNfts { get; set; } // Add this for UserNfts

        public DbSet<Records> LogRecord { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map UserNft to the "UserNfts" table
            modelBuilder.Entity<UsersNft>().ToTable("UserNfts");

            // Configure relationships if necessary
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.User) // Navigation property
                .WithMany()           // No inverse navigation
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Handle cascade delete if needed
        }
    }
}
