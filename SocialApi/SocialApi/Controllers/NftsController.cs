﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;

namespace SocialApi.Controllers
{

    //GET ALL LOGS
    //GET: https://localhost:7279/api/Logs
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class NftsController : ControllerBase
    {
        private readonly WebSocialDbContext dbContext;
        public NftsController(WebSocialDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            { //Get Data From Database - Domain Models
                var nfts = dbContext.Nfts.ToList();

                //Map Domain Models to DTO's
                var nftDto = new List<NftDto>();
                foreach (var nft in nfts)
                {
                    nftDto.Add(new NftDto()
                    {
                        NftId = nft.NftId,
                        NftAddress = nft.NftAddress,
                        Views = nft.Views,
                        Likes = nft.Likes,
                        IsNsfw = nft.IsNsfw,
                        CreatedOn = nft.CreatedOn
                    });
                }
                //Return DTO's
                return Ok(nftDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }




        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var toDb = dbContext.Nfts.FirstOrDefault(x => x.NftId == id);

                if (toDb == null)
                {
                    // Return NotFoundObjectResult if the NFT does not exist
                    return NotFound($"Nft with ID {id} not found.");
                }

                var nftDto = new NftDto
                {
                    NftId = toDb.NftId,
                    NftAddress = toDb.NftAddress,
                    Views = toDb.Views,
                    Likes = toDb.Likes,
                    IsNsfw = toDb.IsNsfw,
                    CreatedOn = toDb.CreatedOn
                };

                return Ok(nftDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handles other errors
            }
        }

        [HttpGet]
        [Route("page-nfts")]
        public IActionResult GetPageNfts(int page, int pageSize, string orderBy = "Likes", bool? isNsfw = null)
        {
            try
            {
                
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest("Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0.");
                }

                // Validate orderBy parameter
                if (orderBy != "Likes" && orderBy != "Views" && orderBy != "CreatedOn")
                {
                    return BadRequest("Invalid input: 'orderBy' must be 'Likes', 'Views', or 'CreatedOn'.");
                }

                // Base query
                var nftQuery = dbContext.Nfts.AsQueryable();

                // Apply filtering for IsNsfw if provided
                if (isNsfw.HasValue)
                {
                    nftQuery = nftQuery.Where(x => x.IsNsfw == isNsfw.Value);
                }

                // Dynamic ordering based on the orderBy parameter
                switch (orderBy)
                {
                    case "Likes":
                        nftQuery = nftQuery.OrderByDescending(x => x.Likes);
                        break;

                    case "Views":
                        nftQuery = nftQuery.OrderByDescending(x => x.Views);
                        break;

                    case "CreatedOn":
                        nftQuery = nftQuery.OrderByDescending(x => x.CreatedOn);
                        break;
                }

                // Total number of filtered nft
                int total = nftQuery.Count();
                int totalPages = (int)Math.Ceiling(total / (double)pageSize); // Calculate total pages

                // Calculate the starting index for the page
                int skip = page * pageSize;

                // Apply pagination and projection
                var nft = nftQuery
                    .Skip(skip) // Skip items for previous pages
                    .Take(pageSize) // Take items for the current page
                    .Select(x => new
                    {
                        nftId = x.NftId, // i may have broken it . NftId goes grat but it was working fine.
                        nftAddress = x.NftAddress,
                        views = x.Views,
                        likes = x.Likes,
                        isNsfw = x.IsNsfw,
                        createdOn = x.CreatedOn
                    })
                    .ToList(); // Convert the result to a list

                // Create the response object
                var result = new
                {
                    Nft = nft,
                    From = skip + 1, // First item index of this page
                    To = Math.Min(skip + nft.Count, total), // Last item index of this page
                    Total = total, // Total number of nfts
                    Pages = totalPages - 1 // Total number of pages
                };

                return Ok(result); // Return the result object
            }
            catch (Exception ex2)
            {
                return BadRequest(ex2.Message); // Handle any exceptions
            }
        }

        [HttpPost]
        public IActionResult CreateNft([FromBody] AddNftRequestDto nftToAdd)
        {
            try
            {
                // Parse Views
                if (!long.TryParse(nftToAdd.Views, out var parsedViews))
                {
                    return BadRequest($"Invalid value for Views: {nftToAdd.Views}");
                }

                // Parse Likes
                if (!long.TryParse(nftToAdd.Likes, out var parsedLikes))
                {
                    return BadRequest($"Invalid value for Likes: {nftToAdd.Likes}");
                }

                // Create the domain model
                var nftDomainModel = new Nfts
                {
                    NftAddress = nftToAdd.NftAddress,
                    Views = parsedViews,
                    Likes = parsedLikes,
                    IsNsfw = nftToAdd.IsNsfw,
                    CreatedOn = nftToAdd.CreatedOn
                };

                dbContext.Add(nftDomainModel);
                dbContext.SaveChanges();

                // Create the DTO for the response
                var newNftDto = new NftDto
                {
                    NftId = nftDomainModel.NftId,
                    NftAddress = nftDomainModel.NftAddress,
                    Views = nftDomainModel.Views,
                    Likes = nftDomainModel.Likes,
                    IsNsfw = nftDomainModel.IsNsfw,
                    CreatedOn = DateTime.Now // Use current timestamp for CreatedOn
                };

                return CreatedAtAction(nameof(GetById), new { id = newNftDto.NftId }, newNftDto);
            }
            catch (Exception ex3)
            {
                return BadRequest(ex3.Message);
            }
        }

       
        [HttpGet]
        [Route("{isNsfw:bool}")]
        public IActionResult GetAllNsfw(bool isNsfw)
        {
            try
            {
                // Get data from database
                var nft = dbContext.Nfts
                    .Where(nft => nft.IsNsfw == isNsfw)
                    .ToList();

                // Map Domain Models to DTOs
                var nftDto = nft.Select(nft => new NftDto
                {
                    NftId = nft.NftId,
                    NftAddress = nft.NftAddress,
                    Views = nft.Views,
                    Likes = nft.Likes,
                    IsNsfw = nft.IsNsfw,
                    CreatedOn = nft.CreatedOn

                }).ToList();

                // Return DTOs
                return Ok(nftDto);
            }
            catch (Exception ex4)
            {
                return BadRequest(ex4.Message);
            }
        }

        [HttpPost("{id}/add-like")]
        public IActionResult AddLike(int id)
        {
            try
            {
                // Find the Nft by ID
                var nft = dbContext.Nfts.FirstOrDefault(r => r.NftId == id);

                if (nft == null)
                {
                    return NotFound($"Nft with ID {id} not found.");
                }

                // Increment the Likes property
                nft.Likes += 1;

                // Save changes to the database
                dbContext.SaveChanges();

                // Return the updated Nft
                var updatedNftDto = new NftDto
                {
                    NftId = nft.NftId,
                    NftAddress = nft.NftAddress,
                    Views = nft.Views,
                    Likes = nft.Likes,
                    IsNsfw = nft.IsNsfw,
                    CreatedOn = nft.CreatedOn
                };

                return Ok(updatedNftDto);
            }
            catch (Exception ex5)
            {
                return BadRequest($"An error occurred: {ex5.Message}");
            }
        }


        [HttpPost("{id}/remove-like")]
        public IActionResult RemoveLike(int id)
        {
            try
            {
                // Find the nft by ID
                var nft = dbContext.Nfts.FirstOrDefault(r => r.NftId == id);

                if (nft == null)
                {
                    return NotFound($"Nft with ID {id} not found.");
                }

                // Decrement the Likes property, ensuring it doesn't go below zero
                if (nft.Likes > 0)
                {
                    nft.Likes = nft.Likes - 1;
                }

                // Save changes to the database
                dbContext.SaveChanges();

                // Return the updated nft
                var updatedNftDto = new NftDto
                {
                    NftId = nft.NftId,
                    NftAddress = nft.NftAddress,
                    Views = nft.Views,
                    Likes = nft.Likes,
                    IsNsfw = nft.IsNsfw,
                    CreatedOn = nft.CreatedOn
                };

                return Ok(updatedNftDto);
            }
            catch (Exception ex6)
            {
                return BadRequest($"An error occurred: {ex6.Message}");
            }
        }


        [HttpPost("{id}/add-view")]
        public IActionResult AddView(int id)
        {
            try
            {
                // Find the nft by ID
                var nft = dbContext.Nfts.FirstOrDefault(r => r.NftId == id);

                if (nft == null)
                {
                    return NotFound($"Nft with ID {id} not found.");
                }

                // Increment the Likes property
                nft.Views += 1;

                // Save changes to the database
                dbContext.SaveChanges();

                // Return the updated nft
                var updatedNftDto = new NftDto
                {
                    NftId = nft.NftId,
                    NftAddress = nft.NftAddress,
                    Views = nft.Views,
                    Likes = nft.Likes,
                    IsNsfw = nft.IsNsfw,
                    CreatedOn = nft.CreatedOn
                };

                return Ok(updatedNftDto);
            }
            catch (Exception ex7)
            {
                return BadRequest($"An error occurred: {ex7.Message}");
            }
        }

    }
}