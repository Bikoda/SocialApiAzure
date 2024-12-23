using Microsoft.AspNetCore.Authorization;
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
                // Validate input parameters
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0." });
                }

                if (!new[] { "Likes", "Views", "CreatedOn" }.Contains(orderBy))
                {
                    return BadRequest(new { Message = "Invalid input: 'orderBy' must be 'Likes', 'Views', or 'CreatedOn'." });
                }

                // Base query
                var nftQuery = dbContext.Nfts.AsQueryable();

                // Apply filtering for IsNsfw if provided
                if (isNsfw.HasValue)
                {
                    nftQuery = nftQuery.Where(n => n.IsNsfw == isNsfw.Value);
                }

                // Dynamic ordering
                nftQuery = orderBy switch
                {
                    "Likes" => nftQuery.OrderByDescending(n => n.Likes),
                    "Views" => nftQuery.OrderByDescending(n => n.Views),
                    "CreatedOn" => nftQuery.OrderByDescending(n => n.CreatedOn),
                    _ => nftQuery // Default case (should never be hit due to prior validation)
                };

                // Total number of filtered NFTs
                int totalNfts = nftQuery.Count();
                int totalPages = (int)Math.Ceiling(totalNfts / (double)pageSize);

                // Calculate the starting index for the page
                int skip = page * pageSize;

                // Apply pagination and projection
                var nfts = nftQuery
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(n => new
                    {
                        NftId = n.NftId,
                        NftAddress = n.NftAddress,
                        Views = n.Views,
                        Likes = n.Likes,
                        IsNsfw = n.IsNsfw,
                        CreatedOn = n.CreatedOn
                    })
                    .ToList();

                // Create the response object
                var response = new
                {
                    Nfts = nfts,
                    From = skip + 1,
                    To = Math.Min(skip + nfts.Count, totalNfts),
                    Total = totalNfts,
                    TotalPages = totalPages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception (optional: replace with a logging framework)
                return BadRequest(new { Message = "An error occurred while processing the request.", Details = ex.Message });
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