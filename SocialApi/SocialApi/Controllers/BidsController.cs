using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;

namespace SocialApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class BidsController : ControllerBase
    {
        private readonly IWebSocialDbContext _dbContext;

        public BidsController(IWebSocialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddBid(AddBidRequestDto dto)
        {
            // Step 1: Create a new bid
            var bid = new Bids
            {
                NftId = dto.NftId,
                StartAmount = dto.StartAmount,
                EndTime = dto.EndTime,
                CurrentTime = DateTime.UtcNow,
                IsOpen = true
            };

            // Add the bid to the database
            await _dbContext.Bids.AddAsync(bid);
            await _dbContext.SaveChangesAsync();

            // Step 2: Create a corresponding OpenBid entry
            var openBid = new OpenBids
            {
                BidId = bid.BidId,
                NftId = bid.NftId,
                HighestBidder = null, // No highest bidder yet
                CurrentTime = DateTime.UtcNow
            };

            // Add the open bid to the database
            await _dbContext.OpenBids.AddAsync(openBid);
            await _dbContext.SaveChangesAsync();

            // Return the created bid as a response
            return Ok(new BidsDto
            {
                BidId = bid.BidId,
                NftId = bid.NftId,
                StartAmount = bid.StartAmount,
                EndTime = bid.EndTime,
                CurrentTime = bid.CurrentTime,
                IsOpen = bid.IsOpen
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBid(long id)
        {
            var bid = await _dbContext.Bids.FindAsync(id);

            if (bid == null)
            {
                return NotFound();
            }

            return Ok(new BidsDto
            {
                BidId = bid.BidId,
                NftId = bid.NftId,
                StartAmount = bid.StartAmount,
                EndTime = bid.EndTime,
                CurrentTime = bid.CurrentTime,
                IsOpen = bid.IsOpen,
                HighestBidder = bid.HighestBidder,
                Amount = bid.Amount
            });
        }
    }
}