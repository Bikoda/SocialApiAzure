using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly WebSocialDbContext dbContext;

        public UsersController(WebSocialDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Get all users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var records = dbContext.Users.ToList();
                var recordDto = records.Select(record => new UsersDto
                {
                    UserId = record.UserId,
                    Nickname = record.Nickname,
                    Email = record.Email,
                    UserAddress = record.UserAddress,
                    CreatedOn = record.CreatedOn
                }).ToList();

                return Ok(recordDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get user by ID
        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            try
            {
                if (!long.TryParse(id, out var parsedId))
                {
                    return BadRequest($"Invalid UserId: {id}");
                }

                var toDb = dbContext.Users.SingleOrDefault(x => x.UserId == parsedId);
                if (toDb == null)
                {
                    return NotFound($"No user found with UserId: {id}");
                }

                var userDto = new UsersDto
                {
                    UserId = toDb.UserId,
                    Nickname = toDb.Nickname,
                    Email = toDb.Email,
                    UserAddress = toDb.UserAddress,
                    CreatedOn = toDb.CreatedOn
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        // Get user by user address
        [HttpGet("by-address/{userAddress}")]
        public IActionResult GetUserByAddress(string userAddress)
        {
            try
            {
                var toDb = dbContext.Users.SingleOrDefault(x => x.UserAddress == userAddress);
                if (toDb == null)
                {
                    return NotFound($"No user found with user address: {userAddress}");
                }

                var userDto = new UsersDto
                {
                    UserId = toDb.UserId,
                    Nickname = toDb.Nickname,
                    Email = toDb.Email,
                    UserAddress = toDb.UserAddress,
                    CreatedOn = toDb.CreatedOn
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred. Please try again later. {ex.Message}");
            }
        }

        // Create a new user
        [HttpPost]
        public IActionResult CreateUser([FromBody] AddUsersRequestDto usersToAdd)
        {
            try
            {
                // Automatically assign CreatedOn if not present in the request DTO
                var usersDomainModel = new Users
                {
                    Nickname = usersToAdd.Nickname,
                    Email = usersToAdd.Email,
                    UserAddress = usersToAdd.UserAddress,
                    CreatedOn = DateTime.UtcNow // Set CreatedOn to the current UTC time
                };

                dbContext.Add(usersDomainModel);
                dbContext.SaveChanges();

                var newUserDto = new UsersDto
                {
                    UserId = usersDomainModel.UserId,
                    Nickname = usersDomainModel.Nickname,
                    Email = usersDomainModel.Email,
                    UserAddress = usersDomainModel.UserAddress,
                    CreatedOn = usersDomainModel.CreatedOn
                };

                return CreatedAtAction(nameof(GetUserById), new { id = newUserDto.UserId }, newUserDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the user: {ex.Message}");
            }
        }
    }
}
