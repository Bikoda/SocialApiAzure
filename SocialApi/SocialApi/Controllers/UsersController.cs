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
            { //Get Data From Database - Domain Models
                var records = dbContext.LogUser.ToList();

                // Map Domain Models to DTOs
                var recordDto = new List<UsersDto>();
                foreach (var record in records) // Iterate through each record in the list
                {
                    recordDto.Add(new UsersDto()
                    {
                        UserId = record.UserId,               // Access the properties of the individual record
                        Nickname = record.Nickname,
                        Email = record.Email,
                        Address = record.Address,
                        CreatedOn = record.CreatedOn
                    });
                }
                //Return DTO's
                return Ok(recordDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            //var toDb =  dbContext.LogRecord.Find(id);
            try
            {
                var toDb = dbContext.LogUser.FirstOrDefault(x => x.UserId == id);

                var userDto = new UsersDto
                {
                    UserId = toDb.UserId,
                    Nickname = toDb.Nickname,
                    Email = toDb.Email,
                    Address = toDb.Address,
                    CreatedOn = toDb.CreatedOn
                };


                return Ok(userDto);
            }
            catch (Exception ex2)
            {
                return BadRequest(ex2.Message);
            }
        }

        [HttpGet]
        [Route("{address}")]
        public IActionResult GetUserByAddress(string address)
        {
            try
            {
                var toDb = dbContext.LogUser.FirstOrDefault(x => x.Address == address);
                if (toDb == null)
                {
                    return NotFound($"No user found with address: {address}");
                }

                var userDto = new UsersDto
                {
                    UserId = toDb.UserId,
                    Nickname = toDb.Nickname,
                    Email = toDb.Email,
                    Address = toDb.Address,
                    CreatedOn = toDb.CreatedOn
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                // Log exception here (e.g., using a logging framework like Serilog or NLog)
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
                    Address = usersToAdd.Address,
                    CreatedOn = usersToAdd.CreatedOn
                };

                dbContext.Add(usersDomainModel);
                dbContext.SaveChanges();


                var newUserDto = new UsersDto
                {
                    UserId = usersDomainModel.UserId,
                    Nickname = usersDomainModel.Nickname,
                    Email = usersDomainModel.Email,
                    Address = usersDomainModel.Address,
                    CreatedOn = usersDomainModel.CreatedOn

                };

                newUserDto.CreatedOn = DateTime.Now;

                return CreatedAtAction(nameof(GetUserById), new { id = newUserDto.UserId }, newUserDto);
            }
            catch (Exception ex5)
            {
                return BadRequest(ex5.Message);
            }
        }
    }
}
