﻿using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;

namespace SocialApi.Data
{
    public class WebSocialDbContext : DbContext, IWebSocialDbContext
    {
        public WebSocialDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public virtual DbSet<Bids> Bids { get; set; }
        public virtual DbSet<OpenBids> OpenBids { get; set; }
        public virtual DbSet<CloseBids> CloseBids { get; set; }
        public virtual DbSet<BidHistory> BidHistory { get; set; }
        public virtual DbSet<Nfts> Nfts { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersNft> UserNfts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bids relationships
            modelBuilder.Entity<Bids>()
                .HasOne(b => b.Nft)
                .WithMany()
                .HasForeignKey(b => b.NftId);

            modelBuilder.Entity<Bids>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.HighestBidder)
                .OnDelete(DeleteBehavior.SetNull);

            // OpenBids relationships
            modelBuilder.Entity<OpenBids>()
                .HasOne(ob => ob.Bid)
                .WithMany()
                .HasForeignKey(ob => ob.BidId);

            modelBuilder.Entity<OpenBids>()
                .HasOne(ob => ob.Nft)
                .WithMany()
                .HasForeignKey(ob => ob.NftId);

            modelBuilder.Entity<OpenBids>()
                .HasOne(ob => ob.User)
                .WithMany()
                .HasForeignKey(ob => ob.HighestBidder)
                .OnDelete(DeleteBehavior.SetNull);

            // CloseBids relationships
            modelBuilder.Entity<CloseBids>()
                .HasOne(cb => cb.Bid)
                .WithMany()
                .HasForeignKey(cb => cb.BidId);

            modelBuilder.Entity<CloseBids>()
                .HasOne(cb => cb.Nft)
                .WithMany()
                .HasForeignKey(cb => cb.NftId);

            modelBuilder.Entity<CloseBids>()
                .HasOne(cb => cb.User)
                .WithMany()
                .HasForeignKey(cb => cb.HighestBidder)
                .OnDelete(DeleteBehavior.SetNull);

            // BidHistory relationships
            modelBuilder.Entity<BidHistory>()
                .HasOne(bh => bh.Nft)
                .WithMany()
                .HasForeignKey(bh => bh.NftId);

            modelBuilder.Entity<BidHistory>()
                .HasOne(bh => bh.User)
                .WithMany()
                .HasForeignKey(bh => bh.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}