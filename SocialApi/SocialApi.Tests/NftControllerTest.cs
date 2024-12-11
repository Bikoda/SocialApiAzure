using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SocialApi.Tests
{
    public class NftControllerTests
    {
        private WebSocialDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WebSocialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            return new WebSocialDbContext(options);
        }

        [Fact]
        public void GetAllNfts_ReturnsOkResult_WithListOfNfts()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.AddRange(new List<Nfts>
            {
                new Nfts { NftId = 1, NftAddress = "0x123", Views = 100, Likes = 10, IsNsfw = false, CreatedOn = DateTime.Now },
                new Nfts { NftId = 2, NftAddress = "0x456", Views = 200, Likes = 20, IsNsfw = true, CreatedOn = DateTime.Now }
            });
            context.SaveChanges();

            var controller = new NftsController(context);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var nfts = Assert.IsType<List<NftDto>>(okResult.Value);
            Assert.Equal(2, nfts.Count);
        }

        [Fact]
        public void GetNftById_ReturnsOkResult_WhenNftExists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.Add(new Nfts
            {
                NftId = 1,
                NftAddress = "0x123",
                Views = 100,
                Likes = 10,
                IsNsfw = false,
                CreatedOn = DateTime.UtcNow
            });
            context.SaveChanges();

            var controller = new NftsController(context);

            // Act
            var result = controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var nftDto = Assert.IsType<NftDto>(okResult.Value);
            Assert.Equal(1, nftDto.NftId);
            Assert.Equal("0x123", nftDto.NftAddress);
        }

        [Fact]
        public void GetNftById_ReturnsNotFound_WhenNftDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new NftsController(context);

            // Act
            var result = controller.GetById(999); // Non-existent NftId

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void CreateNft_ReturnsCreatedAtActionResult_WithNewNft()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new NftsController(context);

            var newNftRequest = new AddNftRequestDto
            {
                NftAddress = "0x789",
                Views = "150",
                Likes = "15",
                IsNsfw = false
            };

            // Act
            var result = controller.CreateNft(newNftRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var nftDto = Assert.IsType<NftDto>(createdAtActionResult.Value);

            Assert.Equal("0x789", nftDto.NftAddress);
            Assert.Equal(150, nftDto.Views);
            Assert.Equal(15, nftDto.Likes);
        }

        [Fact]
        public void AddLike_ReturnsOkResult_WithUpdatedLikes()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.Add(new Nfts
            {
                NftId = 1,
                NftAddress = "0x123",  // Ensure the required property is set
                Likes = 10,
                Views = 100,
                IsNsfw = false,
                CreatedOn = DateTime.UtcNow
            });
            context.SaveChanges();  // Save to persist the data

            var controller = new NftsController(context);

            // Act
            var result = controller.AddLike(1);  // Adding like to the NFT with NftId = 1

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedNft = Assert.IsType<NftDto>(okResult.Value);
            Assert.Equal(11, updatedNft.Likes);  // Ensure the Likes property is incremented
        }

        [Fact]
        public void RemoveLike_ReturnsOkResult_WithUpdatedLikes()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            // Add an Nft to the in-memory database with all required properties
            context.Nfts.Add(new Nfts
            {
                NftId = 1,
                NftAddress = "0x123",  // Ensure the NftAddress is set
                Likes = 10,
                Views = 100,
                IsNsfw = false,
                CreatedOn = DateTime.UtcNow
            });
            context.SaveChanges();  // Save the changes to persist the entity

            var controller = new NftsController(context);

            // Act
            var result = controller.RemoveLike(1);  // Removing like from the Nft with NftId = 1

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedNft = Assert.IsType<NftDto>(okResult.Value);
            Assert.Equal(9, updatedNft.Likes);  // Ensure the Likes property was decremented
        }
    }
}