using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WebSocialDbContext dbContext;

        public UsersController(WebSocialDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                // Retrieve data from the database
                var records = dbContext.Users.ToList();

                // Map domain models to DTOs
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

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            try
            {
                if (!long.TryParse(id, out var parsedId))
                {
                    return BadRequest($"Invalid UserId: {id}");
                }

                var toDb = dbContext.Users.FirstOrDefault(x => x.UserId == parsedId);
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

        [HttpGet("by-address/{userAddress}")]
        public IActionResult GetUserByAddress(string userAddress)
        {
            try
            {
                var toDb = dbContext.Users.FirstOrDefault(x => x.UserAddress == userAddress);
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
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] AddUsersRequestDto usersToAdd)
        {
            try
            {
                var usersDomainModel = new Users
                {
                    Nickname = usersToAdd.Nickname,
                    Email = usersToAdd.Email,
                    UserAddress = usersToAdd.UserAddress,
                    CreatedOn = usersToAdd.CreatedOn
                };

                dbContext.Add(usersDomainModel);
                dbContext.SaveChanges();

                var newUserDto = new UsersDto
                {
                    UserId = usersDomainModel.UserId,
                    Nickname = usersDomainModel.Nickname,
                    Email = usersDomainModel.Email,
                    UserAddress = usersDomainModel.UserAddress,
                    CreatedOn = DateTime.Now
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
  