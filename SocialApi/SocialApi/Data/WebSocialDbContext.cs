using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SocialApi.Data
{
    public class WebSocialDbContext : DbContext
    {
        public WebSocialDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }




        public DbSet<Users> Users { get; set; } // Existing DbSet for Users

        public DbSet<Nfts> Nfts { get; set; }
        public DbSet<UsersNft> UserNfts { get; set; } // Add this for UserNfts





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

            var usersNftEntity = modelBuilder.Model.FindEntityType(typeof(UsersNft));
            Console.WriteLine(usersNftEntity?.FindNavigation("Nft") != null
                ? "Nft navigation found."
                : "Nft navigation not found.");
        }
    }    
}
