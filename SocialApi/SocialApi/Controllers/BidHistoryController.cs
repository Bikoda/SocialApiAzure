using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;

[ApiController]
[Route("api/[controller]")]

public class BidHistoryController : ControllerBase
{
    private readonly IWebSocialDbContext _dbContext;

    public BidHistoryController(IWebSocialDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // POST: api/BidHistory
    [HttpPost]
    public async Task<IActionResult> AddBidHistory(AddBidHistoryDto dto)
    {
        // Validate input
        if (dto == null || dto.NftId <= 0 || dto.UserId <= 0 || string.IsNullOrEmpty(dto.Amount))
        {
            return BadRequest("Invalid input data. Ensure all fields are populated.");
        }

        // Step 1: Find the associated Bid by NftId
        var bid = await _dbContext.Bids.FirstOrDefaultAsync(b => b.NftId == dto.NftId && b.IsOpen);
        if (bid == null)
        {
            return NotFound($"No active bid found for NFT with ID {dto.NftId}.");
        }

        // Step 2: Check if the Bid has already ended
        if (DateTime.UtcNow > bid.EndTime)
        {
            return BadRequest("Cannot place a bid after the auction has ended.");
        }

        // Step 3: Add a new entry to BidHistory
        var bidHistory = new BidHistory
        {
            NftId = dto.NftId,
            UserId = dto.UserId,
            Amount = dto.Amount,
            CurrentTime = DateTime.UtcNow
        };

        await _dbContext.BidHistory.AddAsync(bidHistory);

        // Step 4: Update the Bids and OpenBids tables (as discussed earlier)
        bid.HighestBidder = dto.UserId;
        bid.Amount = dto.Amount;
        _dbContext.Bids.Update(bid);

        var openBid = await _dbContext.OpenBids.FirstOrDefaultAsync(ob => ob.BidId == bid.BidId);
        if (openBid != null)
        {
            openBid.HighestBidder = dto.UserId;
            openBid.Amount = dto.Amount;
            openBid.CurrentTime = DateTime.UtcNow;
            _dbContext.OpenBids.Update(openBid);
        }

        // Save changes
        await _dbContext.SaveChangesAsync();

        // Return the newly created BidHistory entry
        return Ok(new BidHistoryDto
        {
            BidHistoryId = bidHistory.BidHistoryId,
            NftId = bidHistory.NftId,
            UserId = bidHistory.UserId,
            Amount = bidHistory.Amount,
            CurrentTime = bidHistory.CurrentTime
        });
    }

    // GET: api/BidHistory/GetBidHistoryByNftId
    [HttpGet("GetBidHistoryByNftId")]
    public async Task<IActionResult> GetBidHistoryByNftId(int nftId, bool orderByDescending = false, int page = 0, int pageSize = 10)
    {
        // Validate the input
        if (nftId <= 0)
        {
            return BadRequest("Invalid NFT ID.");
        }

        if (page < 0 || pageSize <= 0)
        {
            return BadRequest(new { Message = "Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0." });
        }

        // Query the database for the bid history of the given NFT
        var bidHistoriesQuery = _dbContext.BidHistory.Where(bh => bh.NftId == nftId);

        // Apply ordering based on the parameter
        var bidHistories = orderByDescending
            ? await bidHistoriesQuery.OrderByDescending(bh => bh.CurrentTime).ToListAsync()
            : await bidHistoriesQuery.OrderBy(bh => bh.CurrentTime).ToListAsync();

        // Check if any records were found
        if (bidHistories == null || !bidHistories.Any())
        {
            return NotFound($"No bid history found for NFT with ID {nftId}.");
        }

        // Return the list of bid histories
        return Ok(bidHistories.Select(bh => new BidHistoryDto
        {
            BidHistoryId = bh.BidHistoryId,
            NftId = bh.NftId,
            UserId = bh.UserId,
            Amount = bh.Amount,
            CurrentTime = bh.CurrentTime
        }));
    }

}