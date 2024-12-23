using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Data; // Your DbContext
using SocialApi.Models.Domain; // Your Models
using SocialApi.Models.DTO; // Your DTOs
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly WebSocialDbContext _context;

        public TagsController(WebSocialDbContext context)
        {
            _context = context;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagsDto>>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TagsDto>> GetTag(long id)
        {
            // Find the tag by ID
            var tag = await _context.Tags.FindAsync(id);

            // If the tag doesn't exist, return 404
            if (tag == null)
            {
                return NotFound();
            }

            // Map the Tags entity to a TagsDto
            var tagDto = new TagsDto
            {
                TagId = tag.TagId,
                Name = tag.Name,
                Description = tag.Description,
                Image = tag.Image,
                External_url = tag.External_url,
                Attributes = tag.Attributes
            };

            // Return the TagsDto instead of the Tags entity
            return tagDto;
        }

        // POST: api/Tags
        [HttpPost]
        public async Task<ActionResult<TagsDto>> PostTag(TagsDto tagDto)
        {
            // Create a new Tag from the provided data
            var tag = new Tags
            {
                Name = tagDto.Name,
                Description = tagDto.Description,
                Image = tagDto.Image,
                External_url = tagDto.External_url,
                Attributes = tagDto.Attributes
            };

            // Add the new tag to the database
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            // Return the created tag without the TagId as it is auto-generated
            return CreatedAtAction("GetTag", new { id = tag.TagId }, tagDto);
        }

        // PUT: api/Tags/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(long id, TagsDto tagDto)
        {
            if (id != tagDto.TagId)
            {
                return BadRequest();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            // Update the tag with the new values
            tag.Name = tagDto.Name;
            tag.Description = tagDto.Description;
            tag.Image = tagDto.Image;
            tag.External_url = tagDto.External_url;
            tag.Attributes = tagDto.Attributes;

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
    }
}
