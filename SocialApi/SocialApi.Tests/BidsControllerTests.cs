using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using Xunit;

public class BidsControllerTests
{
    private readonly Mock<IWebSocialDbContext> _mockDbContext;
    private readonly BidsController _controller;
    private readonly List<Bids> _bidsData;
    private readonly List<OpenBids> _openBidsData;

    public BidsControllerTests()
    {
        // Mock DbContext
        _mockDbContext = new Mock<IWebSocialDbContext>();

        // Initialize in-memory data
        _bidsData = new List<Bids>();
        _openBidsData = new List<OpenBids>();

        // Setup DbContext to use in-memory data
        _mockDbContext.Setup(m => m.Bids).Returns(MockDbSet(_bidsData).Object);
        _mockDbContext.Setup(m => m.OpenBids).Returns(MockDbSet(_openBidsData).Object);

        // Initialize the controller with the mocked DbContext
        _controller = new BidsController(_mockDbContext.Object);
    }

    [Fact]
    public async Task AddBid_ReturnsOkResult_WhenBidIsAddedSuccessfully()
    {
        // Arrange: Input DTO
        var dto = new AddBidRequestDto
        {
            NftId = 1,
            StartAmount = "100",
            EndTime = DateTime.UtcNow.AddMinutes(10)
        };

        // Act: Call the AddBid method
        var result = await _controller.AddBid(dto);

        // Assert: Verify results
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBid = Assert.IsType<BidsDto>(okResult.Value);

        // Check that the bid was added
        Assert.Single(_bidsData);
        var addedBid = _bidsData.First();
        Assert.Equal(dto.NftId, addedBid.NftId);
        Assert.Equal(dto.StartAmount, addedBid.StartAmount);

        // Check that the open bid was added
        Assert.Single(_openBidsData);
        var addedOpenBid = _openBidsData.First();
        Assert.Equal(addedBid.BidId, addedOpenBid.BidId);
    }

    [Fact]
    public async Task GetBid_ReturnsOkResult_WhenBidExists()
    {
        // Arrange: Seed in-memory data
        var bid = new Bids
        {
            BidId = 1,
            NftId = 1,
            StartAmount = "100",
            EndTime = DateTime.UtcNow.AddMinutes(10),
            CurrentTime = DateTime.UtcNow,
            IsOpen = true,
            HighestBidder = 2,
            Amount = "200"
        };
        _bidsData.Add(bid); // Add the bid to the mock data

        // Act: Call the GetBid method
        var result = await _controller.GetBid(1);

        // Assert: Verify results
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBid = Assert.IsType<BidsDto>(okResult.Value);

        Assert.Equal(bid.BidId, returnedBid.BidId);
        Assert.Equal(bid.NftId, returnedBid.NftId);
        Assert.Equal(bid.StartAmount, returnedBid.StartAmount);
        Assert.Equal(bid.HighestBidder, returnedBid.HighestBidder);
        Assert.Equal(bid.Amount, returnedBid.Amount);
    }

    [Fact]
    public async Task GetBid_ReturnsNotFound_WhenBidDoesNotExist()
    {
        // Act: Call the GetBid method with a non-existent ID
        var result = await _controller.GetBid(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    private static Mock<DbSet<T>> MockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        // Mock AddAsync
        mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), default)).Callback<T, System.Threading.CancellationToken>((entity, _) => data.Add(entity));
        mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(data.Add);

        // Mock Remove
        mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(entity => data.Remove(entity));

        // Mock FindAsync
        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((object[] keyValues) =>
            {
                var key = keyValues.FirstOrDefault();
                if (key == null) return null;

                return data.SingleOrDefault(entity =>
                {
                    var property = typeof(T).GetProperty("BidId"); // Assuming the key property is "BidId"
                    if (property == null) return false;

                    var value = property.GetValue(entity);
                    return value != null && value.Equals(key);
                });
            });

        return mockSet;
    }
}