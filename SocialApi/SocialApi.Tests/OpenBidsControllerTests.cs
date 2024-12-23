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
    public class OpenBidsControllerTests
    {
        private WebSocialDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WebSocialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            return new WebSocialDbContext(options);
        }

        private OpenBidsController CreateController(WebSocialDbContext context)
        {
            var logger = Mock.Of<ILogger<OpenBidsController>>(); // Mock logger dependency
            return new OpenBidsController(context, logger);
        }

        [Fact]
        public async Task GetAllOpenBids_ReturnsOkResult_WithListOfOpenBids()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            // Add related entities
            context.Bids.Add(new Bids { BidId = 1L, StartAmount = "100" });
            context.Bids.Add(new Bids { BidId = 2L, StartAmount = "200" });

            context.Nfts.Add(new Nfts { NftId = 101L, NftAddress = "0xABCDEF123" });
            context.Nfts.Add(new Nfts { NftId = 102L, NftAddress = "0x123456789" });

            context.Users.Add(new Users
            {
                UserId = 201L,
                Email = "user1@example.com",
                Nickname = "UserOne",
                UserAddress = "0xUSERONEADDRESS"
            });

            context.Users.Add(new Users
            {
                UserId = 202L,
                Email = "user2@example.com",
                Nickname = "UserTwo",
                UserAddress = "0xUSERTWOADDRESS"
            });

            context.OpenBids.AddRange(new List<OpenBids>
            {
                new OpenBids { OpenBidId = 1L, BidId = 1L, NftId = 101L, HighestBidder = 201L, Amount = "100", CurrentTime = DateTime.UtcNow },
                new OpenBids { OpenBidId = 2L, BidId = 2L, NftId = 102L, HighestBidder = 202L, Amount = "200", CurrentTime = DateTime.UtcNow }
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetAllOpenBids();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var openBids = Assert.IsType<List<OpenBidsDto>>(okResult.Value);

            Assert.Equal(2, openBids.Count);
            Assert.Contains(openBids, ob => ob.Amount == "100");
            Assert.Contains(openBids, ob => ob.Amount == "200");
        }

        [Fact]
        public async Task GetOpenBid_ReturnsOkResult_WhenOpenBidExists()
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

            context.OpenBids.Add(new OpenBids
            {
                OpenBidId = 1L,
                BidId = 1L,
                NftId = 101L,
                HighestBidder = 201L,
                Amount = "100",
                CurrentTime = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetOpenBid(1L);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var openBid = Assert.IsType<OpenBidsDto>(okResult.Value);

            Assert.Equal(1L, openBid.OpenBidId);
            Assert.Equal(1L, openBid.BidId);
            Assert.Equal(101L, openBid.NftId);
            Assert.Equal(201L, openBid.HighestBidder);
            Assert.Equal("100", openBid.Amount);
        }

        


        [Fact]
        public async Task DeleteOpenBid_DeletesOpenBidAndReturnsNoContent()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.OpenBids.Add(new OpenBids
            {
                OpenBidId = 1L,
                BidId = 1,
                NftId = 101,
                HighestBidder = 201,
                Amount = "100",
                CurrentTime = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.DeleteOpenBid(1L);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.OpenBids.FindAsync(1L));
        }

        [Fact]
        public async Task GetPaginatedOpenBids_ReturnsPaginatedResults()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.OpenBids.Add(new OpenBids { OpenBidId = 1L, BidId = 1L, NftId = 101L, HighestBidder = 201L, Amount = "100" });
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetPaginatedOpenBids(0, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedOpenBidsResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.Single(response.OpenBids);  // Page size = 1
            Assert.Equal(1, response.Total);   // Total items = 1
            Assert.Equal(1, response.TotalPages);  // Total pages = 1
            Assert.Equal(1, response.From);  // First item index of this page
            Assert.Equal(1, response.To);    // Last item index of this page
        }


    }
}
