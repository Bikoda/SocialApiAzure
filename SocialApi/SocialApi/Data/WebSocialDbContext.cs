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
            // Configure UsersNft primary key
            modelBuilder.Entity<UsersNft>()
                .HasKey(un => un.UserRecordId);

            // Configure User navigation
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNfts)
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Record navigation
            modelBuilder.Entity<UsersNft>()
                .HasOne(un => un.Record) // Navigation property
                .WithMany() // No reverse navigation from Records
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
