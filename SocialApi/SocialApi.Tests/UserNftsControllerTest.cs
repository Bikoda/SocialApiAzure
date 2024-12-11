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
using System.Threading.Tasks;
using Xunit;

namespace SocialApi.Tests
{
    public class UserNftsControllerTests
    {
        private WebSocialDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WebSocialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            return new WebSocialDbContext(options);
        }

        [Fact]
        public async Task PairUserWithNft_ReturnsOkResult_WhenPairingIsSuccessful()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            var user = new Users
            {
                UserId = 1,
                UserAddress = "0x123",  // The address required for pairing
                Email = "user@example.com",  // Required field
                Nickname = "TestUser",  // Required field
                CreatedOn = DateTime.UtcNow
            };

            var nft = new Nfts
            {
                NftId = 1,
                NftAddress = "nft-address",
                Views = 100,
                Likes = 10,
                IsNsfw = false,
                CreatedOn = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.Nfts.Add(nft);
            await context.SaveChangesAsync();

            var controller = new UserNftsController(context);
            var dto = new AddUserNftRequestDto
            {
                UserAddress = "0x123",
                NftAddress = "nft-address"
            };

            // Act
            var result = await controller.PairUserWithNft(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PairingResponseDto>(okResult.Value);
            Assert.Equal("NFT paired successfully.", response.Message);
            Assert.Equal(1L, response.UserNftId);
        }

        [Fact]
        public async Task PairUserWithNft_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.Add(new Nfts { NftId = 1, NftAddress = "nft-address" });
            await context.SaveChangesAsync();

            var controller = new UserNftsController(context);

            var dto = new AddUserNftRequestDto
            {
                UserAddress = "non-existing-address",
                NftAddress = "nft-address"
            };

            // Act
            var result = await controller.PairUserWithNft(dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No user found with address: non-existing-address", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPairedNfts_ReturnsOkResult_WithPairedNfts()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            var user = new Users
            {
                UserId = 1,
                UserAddress = "0x123",  // The address required for pairing
                Email = "user@example.com",  // Required field
                Nickname = "TestUser",  // Required field
                CreatedOn = DateTime.UtcNow
            };

            var nft = new Nfts
            {
                NftId = 1,
                NftAddress = "nft-address",
                Views = 100,
                Likes = 10,
                IsNsfw = false,
                CreatedOn = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.Nfts.Add(nft);
            context.UserNfts.Add(new UsersNft
            {
                UserId = 1,
                NftId = 1,
                CreatedOn = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var controller = new UserNftsController(context);

            // Act
            var result = await controller.GetPairedNfts("0x123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pairedNfts = Assert.IsType<List<UsersNftDto>>(okResult.Value);
            Assert.Single(pairedNfts);  // Ensure there is exactly one paired NFT

            // Assert that the DTO has correct data
            var pairedNft = pairedNfts.First();
            Assert.Equal(1L, pairedNft.UserId);  // Ensure UserId is correct
            Assert.Equal(1L, pairedNft.NftId);   // Ensure NftId is correct
            Assert.Equal("nft-address", pairedNft.NftAddress);  // Ensure NftAddress is correct
        }

        [Fact]
        public async Task GetPairedNfts_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new UserNftsController(context);

            // Act
            var result = await controller.GetPairedNfts("non-existing-address");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No user found with address: non-existing-address", notFoundResult.Value);
        }

        [Fact]
        public async Task PairUserWithNft_ReturnsBadRequest_WhenInvalidDataIsProvided()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            var user = new Users
            {
                UserId = 1,
                UserAddress = "0x123",
                Email = "user@example.com",  // Required field
                Nickname = "TestUser",  // Required field
                CreatedOn = DateTime.UtcNow
            };

            var nft = new Nfts
            {
                NftId = 1,
                NftAddress = "nft-address",
                Views = 100,
                Likes = 10,
                IsNsfw = false,
                CreatedOn = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.Nfts.Add(nft);
            await context.SaveChangesAsync();

            var controller = new UserNftsController(context);

            var dto = new AddUserNftRequestDto
            {
                UserAddress = "0x123",
                // Leaving NftAddress and NftId null will trigger a BadRequest
            };

            // Act
            var result = await controller.PairUserWithNft(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);

            // Update the expected error message to match the controller's response
            Assert.Equal("Either Nft path or Nft ID must be provided.", errorMessage);
        }
    }
}