using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SocialApi.Tests
{
    public class BidHistoryControllerTests
    {
        private readonly BidHistoryController _controller;
        private readonly Mock<IWebSocialDbContext> _mockContext;

        public BidHistoryControllerTests()
        {
            _mockContext = new Mock<IWebSocialDbContext>();
            _controller = new BidHistoryController(_mockContext.Object);
        }
        /*
        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsPaginatedResults_WhenDataExists()
        {
            // Arrange
            var bidHistories = new List<BidHistory>
            {
                new BidHistory { BidHistoryId = 1, NftId = 101, UserId = 1, Amount = "100", CurrentTime = DateTime.UtcNow.AddMinutes(-1) },
                new BidHistory { BidHistoryId = 2, NftId = 101, UserId = 2, Amount = "150", CurrentTime = DateTime.UtcNow },
                new BidHistory { BidHistoryId = 3, NftId = 101, UserId = 3, Amount = "200", CurrentTime = DateTime.UtcNow.AddMinutes(1) }
            };

            _mockContext.Setup(db => db.BidHistory).Returns(MockDbSetHelper.CreateMockDbSet(bidHistories).Object);

            // Act
            var result = await _controller.GetBidHistoryByNftId(101, page: 0, pageSize: 2);

            // Assert
            var okResult = Assert.IsType<ActionResult<PaginatedBidHistoryResponse>>(result);
            var response = Assert.IsType<PaginatedBidHistoryResponse>(okResult.Value);

            // Assert pagination details
            Assert.Equal(2, response.BidHistories.Count); // 2 items on the first page
            Assert.Equal(3, response.Total); // Total items = 3
            Assert.Equal(2, response.TotalPages); // Total pages = 2
            Assert.Equal(1, response.From); // From = 1 (first item index)
            Assert.Equal(2, response.To); // To = 2 (last item index)
        }*/
        /*
        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsNotFound_WhenNoBidHistoryFound()
        {
            // Arrange
            _mockContext.Setup(db => db.BidHistory).Returns(MockDbSetHelper.CreateMockDbSet(new List<BidHistory>()).Object);

            // Act
            var result = await _controller.GetBidHistoryByNftId(999, page: 0, pageSize: 10); // Non-existing nftId

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No bid history found for NFT with ID 999.", notFoundResult.Value);
        }*/

        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsBadRequest_WhenInvalidNftId()
        {
            // Act
            var result = await _controller.GetBidHistoryByNftId(0, page: 0, pageSize: 10); // Invalid nftId (<= 0)

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid NFT ID.", badRequestResult.Value);
        }
        /*
        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsBadRequest_WhenInvalidPagination()
        {
            // Arrange
            var context = MockDbSetHelper.CreateInMemoryDbContext();  // Ensure the method is properly called
            var controller = new BidHistoryController(context);

            // Act
            var result = await controller.GetBidHistoryByNftId(-1, false, 0, 1); // Invalid page number and explicitly pass orderByDescending

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ExpandoObject>(badRequestResult.Value); // Check for ExpandoObject
            var message = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.Equal("Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0.", message);
        }
        */






        /*
        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsPaginatedResults_WhenDescendingOrderIsRequested()
        {
            // Arrange
            var bidHistories = new List<BidHistory>
            {
                new BidHistory { BidHistoryId = 1, NftId = 101, UserId = 1, Amount = "100", CurrentTime = DateTime.UtcNow.AddMinutes(-1) },
                new BidHistory { BidHistoryId = 2, NftId = 101, UserId = 2, Amount = "150", CurrentTime = DateTime.UtcNow },
                new BidHistory { BidHistoryId = 3, NftId = 101, UserId = 3, Amount = "200", CurrentTime = DateTime.UtcNow.AddMinutes(1) }
            };

            _mockContext.Setup(db => db.BidHistory).Returns(MockDbSetHelper.CreateMockDbSet(bidHistories).Object);

            // Act
            var result = await _controller.GetBidHistoryByNftId(101, page: 0, pageSize: 2, orderByDescending: true); // Paginate in descending order

            // Assert
            var okResult = Assert.IsType<ActionResult<PaginatedBidHistoryResponse>>(result);
            var response = Assert.IsType<PaginatedBidHistoryResponse>(okResult.Value);

            // Assert pagination details and order
            Assert.Equal(2, response.BidHistories.Count); // 2 items on the first page
            Assert.Equal(3, response.Total); // Total items = 3
            Assert.Equal(2, response.TotalPages); // Total pages = 2
            Assert.Equal(1, response.From); // From = 1 (first item index)
            Assert.Equal(2, response.To); // To = 2 (last item index)

            // Ensure the bids are ordered correctly (Descending by CurrentTime)
            Assert.Equal("200", response.BidHistories.First().Amount); // The most recent bid should come first
        }*/
    }
}
