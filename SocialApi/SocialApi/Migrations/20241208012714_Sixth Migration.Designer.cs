﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialApi.Data;

#nullable disable

namespace SocialApi.Migrations
{
    [DbContext(typeof(WebSocialDbContext))]
    [Migration("20241208012714_Sixth Migration")]
    partial class SixthMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SocialApi.Models.Domain.Records", b =>
                {
                    b.Property<long>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RecordId"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsNsfw")
                        .HasColumnType("bit");

                    b.Property<long>("Likes")
                        .HasColumnType("bigint");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Views")
                        .HasColumnType("bigint");

                    b.HasKey("RecordId");

                    b.ToTable("LogRecord");
                });

            modelBuilder.Entity("SocialApi.Models.Domain.Users", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("UserId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nickname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("LogUser");
                });

            modelBuilder.Entity("SocialApi.Models.Domain.UsersNft", b =>
                {
                    b.Property<long>("UserRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("UserRecordId"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("RecordId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("UserRecordId");

                    b.HasIndex("RecordId");

                    b.HasIndex("UserId");

                    b.ToTable("UserNfts");
                });

            modelBuilder.Entity("SocialApi.Models.Domain.UsersNft", b =>
                {
                    b.HasOne("SocialApi.Models.Domain.Records", "Record")
                        .WithMany()
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialApi.Models.Domain.Users", "User")
                        .WithMany("UserNfts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Record");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialApi.Models.Domain.Users", b =>
                {
                    b.Navigation("UserNfts");
                });
#pragma warning restore 612, 618
        }
    }
}
