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
    public class CloseBidsController : ControllerBase
    {
        private readonly IWebSocialDbContext _dbContext;

        public CloseBidsController(IWebSocialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/CloseBids
        [HttpGet]
        public async Task<IActionResult> GetAllClosedBids()
        {
            var closeBids = await _dbContext.CloseBids
                .Include(cb => cb.Bid)
                .Include(cb => cb.Nft)
                .Include(cb => cb.User)
                .ToListAsync();

            var response = closeBids.Select(cb => new CloseBidsDto
            {
                CloseBidId = cb.CloseBidId,
                BidId = cb.BidId,
                NftId = cb.NftId,
                HighestBidder = cb.HighestBidder,
                Amount = cb.Amount,
                CloseTime = cb.CloseTime
            });

            return Ok(response);
        }

        // GET: api/CloseBids/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClosedBid(long id)
        {
            // Retrieve the CloseBid from the database
            var closeBid = await _dbContext.CloseBids
                .FirstOrDefaultAsync(cb => cb.CloseBidId == id);

            // Return NotFound if the CloseBid doesn't exist
            if (closeBid == null)
            {
                return NotFound(new { Message = "Closed bid not found." });
            }

            // Map the CloseBid entity to a DTO
            var response = MapToDto(closeBid);

            // Return the DTO with a 200 OK response
            return Ok(response);
        }

        // Helper function to map CloseBids to CloseBidsDto
        private CloseBidsDto MapToDto(CloseBids closeBid)
        {
            return new CloseBidsDto
            {
                CloseBidId = closeBid.CloseBidId,
                BidId = closeBid.BidId,
                NftId = closeBid.NftId,
                HighestBidder = closeBid.HighestBidder,
                Amount = closeBid.Amount,
                CloseTime = closeBid.CloseTime
            };
        }



        // POST: api/CloseBids
        [HttpPost]
        public async Task<IActionResult> AddClosedBid(CloseBidsDto dto)
        {
            var closedBid = new CloseBids
            {
                BidId = dto.BidId,
                NftId = dto.NftId,
                HighestBidder = dto.HighestBidder,
                Amount = dto.Amount,
                CloseTime = DateTime.UtcNow
            };

            await _dbContext.CloseBids.AddAsync(closedBid);
            await _dbContext.SaveChangesAsync();

            return Ok(new CloseBidsDto
            {
                CloseBidId = closedBid.CloseBidId,
                BidId = closedBid.BidId,
                NftId = closedBid.NftId,
                HighestBidder = closedBid.HighestBidder,
                Amount = closedBid.Amount,
                CloseTime = closedBid.CloseTime
            });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClosedBid(long id)
        {
            var closeBid = await _dbContext.CloseBids.FindAsync(id);
            if (closeBid == null)
            {
                return NotFound();
            }

            _dbContext.CloseBids.Remove(closeBid);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}