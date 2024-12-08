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




        public DbSet<Users> LogUser { get; set; } // Existing DbSet for Users

        public DbSet<Records> LogRecord { get; set; }
        public DbSet<UsersNft> UserNfts { get; set; } // Add this for UserNfts





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure UsersNft primary key
            modelBuilder.Entity<UsersNft>()
                .HasKey(un => un.UserRecordId);

            // Configure Users navigation property
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNfts)
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Records navigation property
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.Record)
                .WithMany()
                .HasForeignKey(un => un.RecordId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

            var usersNftEntity = modelBuilder.Model.FindEntityType(typeof(UsersNft));
            Console.WriteLine(usersNftEntity?.FindNavigation("Record") != null
                ? "Record navigation found."
                : "Record navigation not found.");
        }
    }    
}
