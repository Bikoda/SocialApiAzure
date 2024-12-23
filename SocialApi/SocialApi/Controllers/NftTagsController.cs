using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using static SocialApi.Models.DTO.NftTagsDto;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NftTagsController : ControllerBase
    {
        private readonly WebSocialDbContext dbContext;
        public NftTagsController(WebSocialDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("nfts/tags")]
        public async Task<IActionResult> AddTagsToNft(AddNftTagRequestDto request)
        {
            // Validate that the NFT exists
            var nftExists = await dbContext.Nfts.AnyAsync(n => n.NftId == request.NftId);
            if (!nftExists)
            {
                return BadRequest("NFT not found.");
            }

            // Validate that all tags exist
            var validTags = await dbContext.Tags
                .Where(t => request.TagIds.Contains(t.TagId))
                .Select(t => t.TagId)
                .ToListAsync();

            if (validTags.Count != request.TagIds.Count)
            {
                return BadRequest("One or more tags do not exist.");
            }

            // Add the tags to the NFT
            foreach (var tagId in validTags)
            {
                dbContext.NftTags.Add(new NftTags
                {
                    NftId = request.NftId,
                    TagId = tagId,
                    CreatedOn = DateTime.UtcNow
                });
            }

            await dbContext.SaveChangesAsync();

            return Ok(new { Message = "Tags successfully added to NFT." });
        }
        [HttpGet("nft-tags")]
        public IActionResult GetNftTags(int page, int pageSize, long? nftId = null, long? tagId = null)
        {
            try
            {
                // Validate pagination parameters
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0." });
                }

                // Base query
                var nftTagsQuery = dbContext.NftTags.AsQueryable();

                // Apply filters
                if (nftId.HasValue)
                {
                    nftTagsQuery = nftTagsQuery.Where(nt => nt.NftId == nftId.Value);
                }

                if (tagId.HasValue)
                {
                    nftTagsQuery = nftTagsQuery.Where(nt => nt.TagId == tagId.Value);
                }

                // Total items and pages
                int total = nftTagsQuery.Count();
                int totalPages = (int)Math.Ceiling(total / (double)pageSize);

                // Apply pagination
                var nftTags = nftTagsQuery
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(nt => new NftTagsDto(nt.NftId, nt.TagId, nt.CreatedOn))
                    .ToList();

                // Create response
                var response = new PaginatedNftTagsResponse(
                    nftTags: nftTags,
                    from: page * pageSize + 1,
                    to: Math.Min((page + 1) * pageSize, total),
                    total: total,
                    totalPages: totalPages
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "An error occurred while processing the request.", Details = ex.Message });
            }
        }


        [HttpDelete("nfts/{nftId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagFromNft(long nftId, long tagId)
        {
            // Find the entry in the NftTags table
            var nftTag = await dbContext.NftTags
                .FirstOrDefaultAsync(nt => nt.NftId == nftId && nt.TagId == tagId);

            if (nftTag == null)
            {
                return NotFound("Tag not associated with the specified NFT."); // Return string directly
            }

            // Remove the entry
            dbContext.NftTags.Remove(nftTag);
            await dbContext.SaveChangesAsync();

            return Ok(new { Message = "Tag removed from NFT successfully." });
        }
    }
}
