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
    public class OpenBidsController : ControllerBase
    {
        private readonly IWebSocialDbContext _dbContext;

        public OpenBidsController(IWebSocialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/OpenBids
        [HttpGet]
        public async Task<IActionResult> GetAllOpenBids()
        {
            var openBids = await _dbContext.OpenBids
                .Include(ob => ob.Bid)
                .Include(ob => ob.Nft)
                .Include(ob => ob.User)
                .ToListAsync();

            var response = openBids.Select(ob => new OpenBidsDto
            {
                OpenBidId = ob.OpenBidId,
                BidId = ob.BidId,
                NftId = ob.NftId,
                HighestBidder = ob.HighestBidder,
                Amount = ob.Amount,
                CurrentTime = ob.CurrentTime
            });

            return Ok(response);
        }

        // GET: api/OpenBids/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenBid(long id)
        {
            var openBid = await _dbContext.OpenBids
                .Include(ob => ob.Bid)
                .Include(ob => ob.Nft)
                .Include(ob => ob.User)
                .FirstOrDefaultAsync(ob => ob.OpenBidId == id);

            if (openBid == null)
            {
                return NotFound();
            }

            var response = new OpenBidsDto
            {
                OpenBidId = openBid.OpenBidId,
                BidId = openBid.BidId,
                NftId = openBid.NftId,
                HighestBidder = openBid.HighestBidder,
                Amount = openBid.Amount,
                CurrentTime = openBid.CurrentTime
            };

            return Ok(response);
        }

        // PUT: api/OpenBids/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenBid(long id, [FromBody] OpenBidsDto dto)
        {
            var openBid = await _dbContext.OpenBids.FindAsync(id);
            if (openBid == null)
            {
                return NotFound();
            }

            openBid.HighestBidder = dto.HighestBidder;
            openBid.Amount = dto.Amount;
            openBid.CurrentTime = DateTime.UtcNow;

            _dbContext.OpenBids.Update(openBid);
            await _dbContext.SaveChangesAsync();

            return Ok(new OpenBidsDto
            {
                OpenBidId = openBid.OpenBidId,
                BidId = openBid.BidId,
                NftId = openBid.NftId,
                HighestBidder = openBid.HighestBidder,
                Amount = openBid.Amount,
                CurrentTime = openBid.CurrentTime
            });
        }

        // DELETE: api/OpenBids/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenBid(long id)
        {
            var openBid = await _dbContext.OpenBids.FindAsync(id);
            if (openBid == null)
            {
                return NotFound();
            }

            _dbContext.OpenBids.Remove(openBid);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}