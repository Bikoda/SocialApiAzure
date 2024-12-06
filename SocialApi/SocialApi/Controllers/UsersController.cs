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
                        Id = record.Id,               // Access the properties of the individual record
                        Nickname = record.Nickname,
                        Messages = record.Messages,
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
                var toDb = dbContext.LogUser.FirstOrDefault(x => x.Id == id);

                var userDto = new UsersDto
                {
                    Id = toDb.Id,
                    Nickname = toDb.Nickname,
                    Messages = toDb.Messages,
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

        [HttpPost]
        public IActionResult CreateUser([FromBody] AddUsersRequestDto usersToAdd)
        {
            try
            {
                var usersDomainModel = new Users
                {

                    Nickname = usersToAdd.Nickname,
                    Messages = usersToAdd.Messages,
                    Address = usersToAdd.Address,
                    CreatedOn = usersToAdd.CreatedOn
                };

                dbContext.Add(usersDomainModel);
                dbContext.SaveChanges();


                var newUserDto = new UsersDto
                {
                    Id = usersDomainModel.Id,
                    Nickname = usersDomainModel.Nickname,
                    Messages = usersDomainModel.Messages,
                    Address = usersDomainModel.Address,
                    CreatedOn = usersDomainModel.CreatedOn

                };

                newUserDto.CreatedOn = DateTime.Now;

                return CreatedAtAction(nameof(GetUserById), new { id = newUserDto.Id }, newUserDto);
            }
            catch (Exception ex5)
            {
                return BadRequest(ex5.Message);
            }
        }
    }
}
