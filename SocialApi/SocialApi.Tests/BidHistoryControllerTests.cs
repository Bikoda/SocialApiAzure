using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SocialApi.Tests
{
    public class BidHistoryControllerTests
    {   
        private readonly Mock<IWebSocialDbContext> _mockContext;
        private readonly BidHistoryController _controller;

        public BidHistoryControllerTests()
        {
            _mockContext = new Mock<IWebSocialDbContext>();
            _controller = new BidHistoryController(_mockContext.Object);
        }
        /*
        [Fact]
        public async Task AddBidHistory_ReturnsBadRequest_WhenAuctionHasEnded()
        {
            // Arrange: Mock data
            var bidsData = new List<Bids>
            {
                new Bids { BidId = 1, NftId = 1, IsOpen = true, EndTime = System.DateTime.UtcNow.AddMinutes(-10) } // Auction ended
            };

            var mockBids = MockDbSetHelper.CreateMockDbSet(bidsData);
            var mockBidHistory = MockDbSetHelper.CreateMockDbSet(new List<BidHistory>());

            _mockContext.Setup(c => c.Bids).Returns(mockBids.Object);
            _mockContext.Setup(c => c.BidHistory).Returns(mockBidHistory.Object);

            var dto = new AddBidHistoryDto
            {
                NftId = 1,
                UserId = 1,
                Amount = "100"
            };

            // Act
            var result = await _controller.AddBidHistory(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot place a bid after the auction has ended.", badRequestResult.Value);
        }*/
        /*
        [Fact]
        public async Task AddBidHistory_ReturnsNotFound_WhenNoActiveBidExists()
        {
            // Arrange: Mock data
            var bidsData = new List<Bids>(); // No active bids

            var mockBids = MockDbSetHelper.CreateMockDbSet(bidsData);
            var mockBidHistory = MockDbSetHelper.CreateMockDbSet(new List<BidHistory>());

            _mockContext.Setup(c => c.Bids).Returns(mockBids.Object);
            _mockContext.Setup(c => c.BidHistory).Returns(mockBidHistory.Object);

            var dto = new AddBidHistoryDto
            {
                NftId = 1,
                UserId = 1,
                Amount = "100"
            };

            // Act
            var result = await _controller.AddBidHistory(dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No active bid found for NFT with ID 1.", notFoundResult.Value);
        }*/
        /*
        [Fact]
        public async Task AddBidHistory_ReturnsOkResult_WhenValidBidHistoryIsAdded()
        {
            // Arrange: Mock data
            var bidsData = new List<Bids>
    {
        new Bids { BidId = 1, NftId = 1, IsOpen = true, EndTime = System.DateTime.UtcNow.AddMinutes(10) }
    };

            var bidHistoryData = new List<BidHistory>();
            var mockBids = MockDbSetHelper.CreateMockDbSet(bidsData);
            var mockBidHistory = MockDbSetHelper.CreateMockDbSet(bidHistoryData);

            _mockContext.Setup(c => c.Bids).Returns(mockBids.Object);
            _mockContext.Setup(c => c.BidHistory).Returns(mockBidHistory.Object);

            var dto = new AddBidHistoryDto
            {
                NftId = 1,
                UserId = 1,
                Amount = "100"
            };

            // Act
            var result = await _controller.AddBidHistory(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdHistory = Assert.IsType<BidHistoryDto>(okResult.Value);
            Assert.Equal(dto.NftId, createdHistory.NftId);
            Assert.Equal(dto.UserId, createdHistory.UserId);
            Assert.Equal(dto.Amount, createdHistory.Amount);
        }*/


        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsBidHistoriesInAscendingOrder_WhenOrderByDescendingIsFalse()
        {
            // Arrange: Mock data
            var bidHistoriesData = new List<BidHistory>
            {
                new BidHistory { BidHistoryId = 1, NftId = 1, UserId = 1, Amount = "100", CurrentTime = System.DateTime.UtcNow.AddMinutes(-10) },
                new BidHistory { BidHistoryId = 2, NftId = 1, UserId = 2, Amount = "200", CurrentTime = System.DateTime.UtcNow }
            };

            var mockBidHistory = MockDbSetHelper.CreateMockDbSet(bidHistoriesData);
            _mockContext.Setup(c => c.BidHistory).Returns(mockBidHistory.Object);

            // Act
            var result = await _controller.GetBidHistoryByNftId(1, orderByDescending: false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var bidHistories = Assert.IsAssignableFrom<IEnumerable<BidHistoryDto>>(okResult.Value);
            Assert.Equal(1, bidHistories.First().BidHistoryId); // First entry should have the earliest CurrentTime
        }

        [Fact]
        public async Task GetBidHistoryByNftId_ReturnsNotFound_WhenNoHistoryExistsForNftId()
        {
            // Arrange: Mock data
            var mockBidHistory = MockDbSetHelper.CreateMockDbSet(new List<BidHistory>());

            _mockContext.Setup(c => c.BidHistory).Returns(mockBidHistory.Object);

            // Act
            var result = await _controller.GetBidHistoryByNftId(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No bid history found for NFT with ID 1.", notFoundResult.Value);
        }
    }
}
