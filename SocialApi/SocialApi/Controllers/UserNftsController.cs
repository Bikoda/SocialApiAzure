using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using SocialApi.Data;
using SocialApi.Models.DTO;
using System;
using Microsoft.Extensions.Logging;

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
                return BadRequest("User address cannot be null or empty.");

            if (string.IsNullOrEmpty(dto.Path) && string.IsNullOrEmpty(dto.RecordId))
                return BadRequest("Either Record path or Record ID must be provided.");

            try
            {
                // Find user
                var user = await FindUserByAddressAsync(dto.Address);
                if (user == null)
                    return NotFound($"No user found with address: {dto.Address}");

                // Find record
                var record = await FindRecordByIdOrPathAsync(dto.RecordId, dto.Path);
                if (record == null)
                    return NotFound($"No record found with the given {(string.IsNullOrEmpty(dto.RecordId) ? "Path" : "RecordId")}.");

                // Check and unpair existing NFT
                var existingPairing = await _context.UserNfts.SingleOrDefaultAsync(un => un.RecordId == record.RecordId);
                if (existingPairing != null)
                    _context.UserNfts.Remove(existingPairing);

                // Pair new NFT
                var userNft = new UsersNft
                {
                    UserId = user.UserId,
                    RecordId = record.RecordId,
                    CreatedOn = DateTime.UtcNow
                };

                _context.UserNfts.Add(userNft);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "NFT paired successfully.", UserNftId = userNft.UserRecordId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-paired-nfts")]
        public async Task<IActionResult> GetPairedNfts([FromQuery] string address)
        {
            if (string.IsNullOrEmpty(address))
                return BadRequest("User address cannot be null or empty.");

            try
            {
                // Find user
                var user = await FindUserByAddressAsync(address);
                if (user == null)
                    return NotFound($"No user found with address: {address}");

                // Fetch paired NFTs
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

                return Ok(pairedNfts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Private helper methods


        private async Task<Users> FindUserByAddressAsync(string address)
        {
            return await _context.LogUser.SingleOrDefaultAsync(u => u.Address == address);
        }

        private async Task<Records> FindRecordByIdOrPathAsync(string? recordId, string? path)
        {
            if (!string.IsNullOrEmpty(recordId) && long.TryParse(recordId, out var parsedRecordId))
            {
                return await _context.LogRecord.SingleOrDefaultAsync(r => r.RecordId == parsedRecordId);
            }

            if (!string.IsNullOrEmpty(path))
            {
                return await _context.LogRecord.SingleOrDefaultAsync(r => r.Path == path);
            }

            return null;
        }
    }
}
    