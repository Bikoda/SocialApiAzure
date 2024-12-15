using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using Xunit;

public class CloseBidsControllerTests
{
    [Fact]
    public async Task GetAllClosedBids_ReturnsOkResult_WithClosedBids()
    {
        // Arrange
        var testData = new List<CloseBids>
        {
            new CloseBids { CloseBidId = 1, BidId = 1, NftId = 1, HighestBidder = 1, Amount = "100.00", CloseTime = DateTime.UtcNow },
            new CloseBids { CloseBidId = 2, BidId = 2, NftId = 2, HighestBidder = 2, Amount = "200.00", CloseTime = DateTime.UtcNow }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSet(testData);
        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        var controller = new CloseBidsController(mockDbContext.Object);

        // Act
        var result = await controller.GetAllClosedBids();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBids = Assert.IsAssignableFrom<IEnumerable<CloseBidsDto>>(okResult.Value);
        Assert.Equal(2, returnedBids.Count());
    }
    /*
    [Fact]
    public async Task GetClosedBid_ReturnsOkResult_WhenClosedBidExists()
    {
        // Arrange
        var testData = new List<CloseBids>
        {
            new CloseBids { CloseBidId = 1, BidId = 101, NftId = 5, HighestBidder = 200, Amount = "150.00", CloseTime = DateTime.UtcNow },
            new CloseBids { CloseBidId = 2, BidId = 102, NftId = 6, HighestBidder = 201, Amount = "250.00", CloseTime = DateTime.UtcNow }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSet(testData);
        MockDbSetHelper.SetupFindAsync(mockDbSet, testData); // Ensure FindAsync is properly mocked

        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        var controller = new CloseBidsController(mockDbContext.Object);

        // Act
        var result = await controller.GetClosedBid(1); // ID 1 exists in testData

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Verify that the result is OkObjectResult
        var dto = Assert.IsType<CloseBidsDto>(okResult.Value);

        Assert.Equal(1, dto.CloseBidId); // Verify the correct data is returned
    }*/
    /*
    [Fact]
    public async Task GetClosedBid_ReturnsNotFound_WhenClosedBidDoesNotExist()
    {
        // Arrange: Create test data
        var testData = new List<CloseBids>
    {
        new CloseBids { CloseBidId = 2, BidId = 101, NftId = 5, HighestBidder = 200, Amount = "150.00", CloseTime = DateTime.UtcNow },
        new CloseBids { CloseBidId = 3, BidId = 102, NftId = 6, HighestBidder = 201, Amount = "250.00", CloseTime = DateTime.UtcNow }
    }.AsQueryable();

        // Mock the DbSet
        var mockDbSet = new Mock<DbSet<CloseBids>>();
        mockDbSet.As<IQueryable<CloseBids>>().Setup(m => m.Provider).Returns(testData.Provider);
        mockDbSet.As<IQueryable<CloseBids>>().Setup(m => m.Expression).Returns(testData.Expression);
        mockDbSet.As<IQueryable<CloseBids>>().Setup(m => m.ElementType).Returns(testData.ElementType);
        mockDbSet.As<IQueryable<CloseBids>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((object[] ids) => testData.FirstOrDefault(x => x.CloseBidId == (int)ids[0]));

        // Mock the DbContext
        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        // Create the controller
        var controller = new CloseBidsController(mockDbContext.Object);

        // Act: Call the method with an ID that doesn't exist
        var result = await controller.GetClosedBid(1); // ID 1 does not exist in testData

        // Assert: Verify the NotFound result
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("Closed bid not found.", notFoundResult.Value.ToString());
    }*/





    [Fact]
    public async Task AddClosedBid_ReturnsOkResult_WhenClosedBidIsAdded()
    {
        // Arrange
        var testData = new List<CloseBids>();
        var mockDbSet = MockDbSetHelper.CreateMockDbSet(testData);

        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        var controller = new CloseBidsController(mockDbContext.Object);

        var newBidDto = new CloseBidsDto
        {
            BidId = 1,
            NftId = 1,
            HighestBidder = 1,
            Amount = "100.00"
        };

        // Act
        var result = await controller.AddClosedBid(newBidDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBid = Assert.IsType<CloseBidsDto>(okResult.Value);

        Assert.Equal(newBidDto.BidId, returnedBid.BidId);

        // Verify AddAsync and SaveChanges were called
        mockDbSet.Verify(m => m.AddAsync(It.IsAny<CloseBids>(), It.IsAny<CancellationToken>()), Times.Once);
        mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteClosedBid_ReturnsNoContent_WhenClosedBidIsDeleted()
    {
        // Arrange
        var testData = new List<CloseBids>
    {
        new CloseBids { CloseBidId = 1, BidId = 1, NftId = 1, HighestBidder = 1, Amount = "100.00", CloseTime = DateTime.UtcNow }
    };

        var mockDbSet = MockDbSetHelper.CreateMockDbSet(testData);

        // Configure the FindAsync method to return a valid CloseBids object when the ID exists
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                 .ReturnsAsync((object[] keyValues) =>
                 {
                     var id = (long)keyValues[0];
                     return testData.FirstOrDefault(cb => cb.CloseBidId == id);
                 });

        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        var controller = new CloseBidsController(mockDbContext.Object);

        // Act
        var result = await controller.DeleteClosedBid(1); // Use an ID that exists in testData

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockDbSet.Verify(m => m.Remove(It.IsAny<CloseBids>()), Times.Once); // Ensure Remove was called
        mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Ensure changes are saved
    }




    [Fact]
    public async Task DeleteClosedBid_ReturnsNotFound_WhenClosedBidDoesNotExist()
    {
        // Arrange
        var testData = new List<CloseBids>();
        var mockDbSet = MockDbSetHelper.CreateMockDbSet(testData);
        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        var controller = new CloseBidsController(mockDbContext.Object);

        // Act
        var result = await controller.DeleteClosedBid(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockDbSet.Verify(m => m.Remove(It.IsAny<CloseBids>()), Times.Never);
        mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    /*
    [Fact]
    public async Task GetClosedBid_ReturnsCorrectData_WhenDataExists()
    {
        // Arrange
        var testData = new List<CloseBids>
    {
        new CloseBids { CloseBidId = 1, BidId = 101, NftId = 5, HighestBidder = 200, Amount = "150.00", CloseTime = DateTime.UtcNow },
        new CloseBids { CloseBidId = 2, BidId = 102, NftId = 6, HighestBidder = 201, Amount = "250.00", CloseTime = DateTime.UtcNow }
    };

        var mockDbSet = MockDbSetHelper.CreateMockDbSet(testData);

        var mockDbContext = new Mock<IWebSocialDbContext>();
        mockDbContext.Setup(c => c.CloseBids).Returns(mockDbSet.Object);

        var controller = new CloseBidsController(mockDbContext.Object);

        // Act
        var result = await controller.GetClosedBid(1); // ID 1 exists in testData

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Verify that the result is OkObjectResult
        var dto = Assert.IsType<CloseBidsDto>(okResult.Value);

        Assert.Equal(1, dto.CloseBidId); // Verify the correct data is returned
        Assert.Equal(101, dto.BidId);
        Assert.Equal(5, dto.NftId);
        Assert.Equal(200, dto.HighestBidder);
        Assert.Equal("150.00", dto.Amount);
    }*/




}
