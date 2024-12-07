using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using SocialApi.Data;
using SocialApi.Models.DTO;
using System;

namespace SocialApi.Controllers
{
    
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserNftsController : ControllerBase
    {
        private readonly WebSocialDbContext _context;

        public UserNftsController(WebSocialDbContext context)
        {
            _context = context;
        }
   
        [HttpPost("pair")]
        public async Task<IActionResult> PairUserWithRecord([FromBody] AddUserNftRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.Address))
            {
                return BadRequest("User address cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(dto.Path) && !dto.RecordId.HasValue)
            {
                return BadRequest("Either Record path or Record ID must be provided.");
            }

            // Find the user by address
            var user = await _context.LogUser.SingleOrDefaultAsync(u => u.Address == dto.Address);
            if (user == null)
            {
                return NotFound($"No user found with address: {dto.Address}");
            }

            // Find the record by ID or Path
            var record = dto.RecordId.HasValue
                ? await _context.LogRecord.SingleOrDefaultAsync(r => r.RecordId == dto.RecordId.Value)
                : await _context.LogRecord.SingleOrDefaultAsync(r => r.Path == dto.Path);

            if (record == null)
            {
                return NotFound($"No record found with the given {(dto.RecordId.HasValue ? "RecordId" : "Path")}.");
            }

            // Check if the NFT is already paired and unpair it
            var existingPairing = await _context.UserNfts.SingleOrDefaultAsync(un => un.RecordId == record.RecordId);
            if (existingPairing != null)
            {
                _context.UserNfts.Remove(existingPairing);
            }

            // Create a new UsersNft pairing
            var userNft = new UsersNft
            {
                UserId = user.UserId,
                RecordId = record.RecordId
            };

            // Add and save to the database
            _context.UserNfts.Add(userNft);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "NFT paired successfully.", UserNftId = userNft.UserRecordId });
        }

        [HttpGet("get-paired-nfts")]
        public async Task<IActionResult> GetPairedNfts([FromQuery] string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest("User address cannot be null or empty.");
            }

            // Find the user by address
            var user = await _context.LogUser.SingleOrDefaultAsync(u => u.Address == address);

            if (user == null)
            {
                return NotFound($"No user found with address: {address}");
            }

            // Get the paired NFTs for the user
            var pairedNfts = await _context.UserNfts
                .Where(un => un.UserId == user.UserId)
                .Join(_context.LogRecord,
                      un => un.RecordId,
                      record => record.RecordId,
                      (un, record) => new
                      {
                          RecordId = record.RecordId,
                          Path = record.Path
                      })
                .ToListAsync();

            // Return the results
            return Ok(pairedNfts);
        }
    }

}


