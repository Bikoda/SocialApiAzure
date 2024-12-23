using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class CloseBidsControllerTests
    {
        private WebSocialDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WebSocialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            return new WebSocialDbContext(options);
        }

        private CloseBidsController CreateController(WebSocialDbContext context)
        {
            // Mock the logger dependency
            var logger = Mock.Of<ILogger<CloseBidsController>>();

            // Return a new instance of CloseBidsController with both dependencies
            return new CloseBidsController(context, logger);
        }

       


        [Fact]
        public async Task GetClosedBid_ReturnsOkResult_WhenCloseBidExists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            context.Bids.Add(new Bids { BidId = 1L, StartAmount = "100" });
            context.Nfts.Add(new Nfts { NftId = 101L, NftAddress = "0xABCDEF123" });
            context.Users.Add(new Users
            {
                UserId = 201L,
                Email = "user1@example.com",
                Nickname = "UserOne",
                UserAddress = "0xUSERONEADDRESS"
            });

            context.CloseBids.Add(new CloseBids
            {
                CloseBidId = 1L,
                BidId = 1L,
                NftId = 101L,
                HighestBidder = 201L,
                Amount = "100",
                CloseTime = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetClosedBid(1L);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var closeBid = Assert.IsType<CloseBidsDto>(okResult.Value);

            Assert.Equal(1L, closeBid.CloseBidId);
            Assert.Equal(1L, closeBid.BidId);
            Assert.Equal(101L, closeBid.NftId);
            Assert.Equal(201L, closeBid.HighestBidder);
            Assert.Equal("100", closeBid.Amount);
        }

        [Fact]
        public async Task GetClosedBid_ReturnsNotFound_WhenCloseBidDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();  // Set up your in-memory context
            var controller = CreateController(context);

            // Act
            var result = await controller.GetClosedBid(999L);  // Non-existent ID

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);  // Expecting NotFoundResult without content
        }


        [Fact]
        public async Task GetPaginatedClosedBids_ReturnsPaginatedResults()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            // Add necessary data to the context
            context.CloseBids.AddRange(new CloseBids { CloseBidId = 1, BidId = 1, NftId = 101, HighestBidder = 201, Amount = "100", CloseTime = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetPaginatedClosedBids(0, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CloseBidsPaginatedResponse>(okResult.Value);  // Expecting CloseBidsPaginatedResponse instead of PaginatedCloseBidsResponse

            Assert.NotNull(response);
            Assert.Single(response.CloseBids);  // Page size = 1
            Assert.Equal(1, response.Total);   // Total items = 1
            Assert.Equal(1, response.TotalPages);  // Total pages = 1
            Assert.Equal(1, response.From);   // First item index of this page
            Assert.Equal(1, response.To);     // Last item index of this page
        }

    }
}
