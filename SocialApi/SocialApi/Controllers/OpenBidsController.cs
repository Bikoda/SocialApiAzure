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
        private readonly ILogger<OpenBidsController> _logger;

        public OpenBidsController(IWebSocialDbContext dbContext, ILogger<OpenBidsController> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Helper function to include necessary relationships
        private IQueryable<OpenBids> IncludeBidRelations(IQueryable<OpenBids> query)
        {
            return query.Include(ob => ob.Bid)
                        .Include(ob => ob.Nft)
                        .Include(ob => ob.User);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenBids()
        {
            try
            {
                var openBids = await _dbContext.OpenBids
                    .Include(ob => ob.Bid)
                    .Include(ob => ob.Nft)
                    .Include(ob => ob.User)
                    .ToListAsync();

                var response = openBids
                    .Select(ob => new OpenBidsDto
                    {
                        OpenBidId = ob.OpenBidId,
                        BidId = ob.BidId,
                        NftId = ob.NftId,
                        HighestBidder = ob.HighestBidder,
                        Amount = ob.Amount,
                        CurrentTime = ob.CurrentTime
                    })
                    .ToList();

                return Ok(response);  // Return OkObjectResult explicitly
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllOpenBids: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

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
                return NotFound();  // This returns a NotFoundResult, wrapped in ActionResult<OpenBidsDto>
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


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenBid(long id, [FromBody] OpenBidsDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Amount))
            {
                return BadRequest(new { Message = "Invalid request data." });
            }

            try
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

                _logger.LogInformation($"Successfully updated open bid with ID: {id}");
                return NoContent(); // No content is a good response for a successful update.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UpdateOpenBid: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenBid(long id)
        {
            try
            {
                var openBid = await _dbContext.OpenBids.FindAsync(id);
                if (openBid == null)
                {
                    return NotFound();
                }

                _dbContext.OpenBids.Remove(openBid);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Successfully deleted open bid with ID: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteOpenBid: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedOpenBids(int page = 0, int pageSize = 10)
        {
            try
            {
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0." });
                }

                var query = _dbContext.OpenBids
                    .Include(ob => ob.Bid)
                    .Include(ob => ob.Nft)
                    .Include(ob => ob.User)
                    .AsQueryable();

                int totalItems = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var openBids = await query
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(ob => new OpenBidsDto
                    {
                        OpenBidId = ob.OpenBidId,
                        BidId = ob.BidId,
                        NftId = ob.NftId,
                        HighestBidder = ob.HighestBidder,
                        Amount = ob.Amount,
                        CurrentTime = ob.CurrentTime
                    })
                    .ToListAsync();

                var response = new PaginatedOpenBidsResponse
                {
                    OpenBids = openBids,
                    From = page * pageSize + 1,
                    To = Math.Min((page + 1) * pageSize, totalItems),
                    Total = totalItems,
                    TotalPages = totalPages
                };

                return Ok(response);  // Return OkObjectResult explicitly
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPaginatedOpenBids: {Message}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

