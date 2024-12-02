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

    public class RecordsController : ControllerBase
    {
        private readonly WebSocialDbContext dbContext;
        public RecordsController(WebSocialDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            { //Get Data From Database - Domain Models
                var records = dbContext.LogRecord.ToList();

                //Map Domain Models to DTO's
                var recordDto = new List<RecordsDto>();
                foreach (var record in records)
                {
                    recordDto.Add(new RecordsDto()
                    {
                        Id = record.Id,
                        Path = record.Path,
                        Views = record.Views,
                        Likes = record.Likes,
                        IsNsfw = record.IsNsfw,
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
        public IActionResult GetById(int id)
        {
            //var toDb =  dbContext.LogRecord.Find(id);
            try
            {
                var toDb = dbContext.LogRecord.FirstOrDefault(x => x.Id == id);

                var recordDto = new RecordsDto
                {
                    Id = toDb.Id,
                    Path = toDb.Path,
                    Views = toDb.Views,
                    Likes = toDb.Likes,
                    IsNsfw = toDb.IsNsfw,
                    CreatedOn = toDb.CreatedOn
                };


                return Ok(recordDto);
            }
            catch (Exception ex2)
            {
                return BadRequest(ex2.Message);
            }
        }
        [HttpGet]
        [Route("top-views")]
        public IActionResult GetTopViewed()
        {
            try
            {
                var topItems = dbContext.LogRecord
                    .OrderByDescending(x => x.Views) // Order by Views in descending order
                    .Take(50) // Take the top 50 items
                    .Select(x => new RecordsDto
                    {
                        Id = x.Id,
                        Path = x.Path,
                        Views = x.Views,
                        Likes = x.Likes,
                        IsNsfw = x.IsNsfw,
                        CreatedOn = x.CreatedOn
                    })
                    .ToList(); // Convert the result to a list

                return Ok(topItems); // Return the result
            }
            catch (Exception ex3)
            {
                return BadRequest(ex3.Message); // Handle any exceptions
            }
        }

        [HttpGet]
        [Route("top-likes")]
        public IActionResult GetTopLiked()
        {
            try
            {
                var topItems = dbContext.LogRecord
                    .OrderByDescending(x => x.Likes) // Order by Views in descending order
                    .Take(50) // Take the top 50 items
                    .Select(x => new RecordsDto
                    {
                        Id = x.Id,
                        Path = x.Path,
                        Views = x.Views,
                        Likes = x.Likes,
                        IsNsfw = x.IsNsfw,
                        CreatedOn = x.CreatedOn
                    })
                    .ToList(); // Convert the result to a list

                return Ok(topItems); // Return the result
            }
            catch (Exception ex4)
            {
                return BadRequest(ex4.Message); // Handle any exceptions
            }
        }

        [HttpPost]
        public IActionResult CreateRecord([FromBody] AddRecordsRequestDto recordsToAdd)
        {
            try
            {
                var recordsDomainModel = new Records
                {

                    Path = recordsToAdd.Path,
                    Views = recordsToAdd.Views,
                    Likes = recordsToAdd.Likes,
                    IsNsfw = recordsToAdd.IsNsfw,
                    CreatedOn = recordsToAdd.CreatedOn
                };

                dbContext.Add(recordsDomainModel);
                dbContext.SaveChanges();


                var newRecordsDto = new RecordsDto
                {
                    Id = recordsDomainModel.Id,
                    Path = recordsDomainModel.Path,
                    Views = recordsDomainModel.Views,
                    Likes = recordsDomainModel.Likes,
                    IsNsfw = recordsDomainModel.IsNsfw,
                    CreatedOn = recordsToAdd.CreatedOn

                };

                newRecordsDto.CreatedOn = DateTime.Now;

                return CreatedAtAction(nameof(GetById), new { id = newRecordsDto.Id }, newRecordsDto);
            }
            catch (Exception ex5)
            {
                return BadRequest(ex5.Message);
            }
        }

        [HttpGet]
        [Route("{isNsfw:bool}")]
        public IActionResult GetAllNsfw(bool isNsfw)
        {
            try
            {
                // Get data from database
                var records = dbContext.LogRecord
                    .Where(record => record.IsNsfw == isNsfw)
                    .ToList();

                // Map Domain Models to DTOs
                var recordDto = records.Select(record => new RecordsDto
                {
                    Id = record.Id,
                    Path = record.Path,
                    Views = record.Views,
                    Likes = record.Likes,
                    IsNsfw = record.IsNsfw,
                    CreatedOn = record.CreatedOn

                }).ToList();

                // Return DTOs
                return Ok(recordDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}