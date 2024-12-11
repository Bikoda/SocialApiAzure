﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using SocialApi.Data;
using SocialApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace SocialApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserNftsController : ControllerBase
    {
        

        private readonly WebSocialDbContext _context;

        public UserNftsController(WebSocialDbContext context)
        {
            _context = context;
        }

        [HttpPost("pair")]
        public async Task<IActionResult> PairUserWithNft([FromBody] AddUserNftRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserAddress))
                return BadRequest("User address cannot be null or empty.");

            if (string.IsNullOrEmpty(dto.NftAddress) && string.IsNullOrEmpty(dto.NftId))
                return BadRequest("Either Nft path or Nft ID must be provided.");

            try
            {
                // Find user
                var user = await FindUserByAddressAsync(dto.UserAddress);
                if (user == null)
                    return NotFound($"No user found with address: {dto.UserAddress}");

                // Find record
                var nft = await FindRecordByIdOrPathAsync(dto.NftId, dto.NftAddress);
                if (nft == null)
                    return NotFound($"No record found with the given {(string.IsNullOrEmpty(dto.NftId) ? "NftAddress" : "NftId")}.");

                // Check and unpair existing NFT
                var existingPairing = await _context.UserNfts.SingleOrDefaultAsync(un => un.NftId == nft.NftId);
                if (existingPairing != null)
                    _context.UserNfts.Remove(existingPairing);

                // Pair new NFT
                var userNft = new UsersNft
                {
                    UserId = user.UserId,
                    NftId = nft.NftId,
                    CreatedOn = DateTime.UtcNow
                };

                _context.UserNfts.Add(userNft);
                await _context.SaveChangesAsync();

                var response = new PairingResponseDto("NFT paired successfully.", userNft.UserNftId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-paired-nfts")]
        public async Task<IActionResult> GetPairedNfts([FromQuery] string userAddress)
        {
            if (string.IsNullOrEmpty(userAddress))
                return BadRequest("User address cannot be null or empty.");

            try
            {
                // Find user
                var user = await FindUserByAddressAsync(userAddress);
                if (user == null)
                    return NotFound($"No user found with address: {userAddress}");

                // Fetch paired NFTs
                var pairedNfts = await _context.UserNfts
                    .Where(un => un.UserId == user.UserId)
                    .Join(_context.Nfts,
                          un => un.NftId,
                          nft => nft.NftId,
                          (un, nft) => new UsersNftDto(
                              un.UserNftId,
                              un.UserId,
                              nft.NftId,
                              un.CreatedOn,
                              nft.NftAddress // Pass the NftAddress from Nfts table
                          ))
                    .ToListAsync();

                return Ok(pairedNfts);
            }
            catch (Exception ex1)
            {
                return StatusCode(500, $"An error occurred: {ex1.Message}");
            }
        }
        // Private helper methods


        private async Task<Users> FindUserByAddressAsync(string userAddress)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserAddress == userAddress);
        }

        private async Task<Nfts> FindRecordByIdOrPathAsync(string? nftId, string? nftAddress)
        {
            if (!string.IsNullOrEmpty(nftId) && long.TryParse(nftId, out var parsedNftId))
            {
                return await _context.Nfts.SingleOrDefaultAsync(r => r.NftId == parsedNftId);
            }

            if (!string.IsNullOrEmpty(nftAddress))
            {
                return await _context.Nfts.SingleOrDefaultAsync(r => r.NftAddress == nftAddress);
            }

            return null;
        }
    }
}
    