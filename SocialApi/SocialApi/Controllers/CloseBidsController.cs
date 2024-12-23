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
        private readonly ILogger<CloseBidsController> _logger;
        private readonly IWebSocialDbContext _dbContext;

        // Combine both constructors into one
        public CloseBidsController(IWebSocialDbContext dbContext, ILogger<CloseBidsController> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClosedBid(long id)
        {
            var closeBid = await _dbContext.CloseBids
                .FirstOrDefaultAsync(cb => cb.CloseBidId == id);

            if (closeBid == null)
            {
                return NotFound();  // Return NotFoundResult without any body content
            }

            var response = new CloseBidsDto
            {
                CloseBidId = closeBid.CloseBidId,
                BidId = closeBid.BidId,
                NftId = closeBid.NftId,
                HighestBidder = closeBid.HighestBidder,
                Amount = closeBid.Amount,
                CloseTime = closeBid.CloseTime
            };

            return Ok(response);
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedClosedBids(int page = 0, int pageSize = 10)
        {
            try
            {
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0." });
                }

                var query = _dbContext.CloseBids
                    .Include(cb => cb.Bid)
                    .Include(cb => cb.Nft)
                    .Include(cb => cb.User)
                    .AsQueryable();

                int totalItems = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var closeBids = await query
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(cb => new CloseBidsDto
                    {
                        CloseBidId = cb.CloseBidId,
                        BidId = cb.BidId,
                        NftId = cb.NftId,
                        HighestBidder = cb.HighestBidder,
                        Amount = cb.Amount,
                        CloseTime = cb.CloseTime
                    })
                    .ToListAsync();

                var response = new CloseBidsPaginatedResponse
                {
                    CloseBids = closeBids,
                    From = page * pageSize + 1,
                    To = Math.Min((page + 1) * pageSize, totalItems),
                    Total = totalItems,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {MethodName}: {Message}", nameof(GetPaginatedClosedBids), ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

               // GET: api/CloseBids/nft/{nftId}
        [HttpGet("nft/{nftId}")]
        public async Task<IActionResult> GetClosedBidsByNftId(long nftId, int page = 0, int pageSize = 10)
        {
            if (page < 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0." });
            }

            try
            {
                var query = _dbContext.CloseBids
                    .Include(cb => cb.Bid)
                    .Include(cb => cb.Nft)
                    .Include(cb => cb.User)
                    .Where(cb => cb.NftId == nftId)
                    .AsQueryable();

                int totalItems = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var closeBids = await query
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(cb => new CloseBidsDto
                    {
                        CloseBidId = cb.CloseBidId,
                        BidId = cb.BidId,
                        NftId = cb.NftId,
                        HighestBidder = cb.HighestBidder,
                        Amount = cb.Amount,
                        CloseTime = cb.CloseTime
                    })
                    .ToListAsync();

                var response = new PaginatedCloseBidsResponse
                {
                    CloseBids = closeBids,
                    From = page * pageSize + 1,
                    To = Math.Min((page + 1) * pageSize, totalItems),
                    Total = totalItems,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
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
    }

    // Paginated response model for CloseBids
    public class CloseBidsPaginatedResponse
    {
        public List<CloseBidsDto> CloseBids { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}
