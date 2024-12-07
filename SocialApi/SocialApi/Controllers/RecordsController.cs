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
                        RecordId = record.RecordId,
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
                var toDb = dbContext.LogRecord.FirstOrDefault(x => x.RecordId == id);

                var recordDto = new RecordsDto
                {
                    RecordId = toDb.RecordId,
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
                        RecordId = x.RecordId,
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
                        RecordId = x.RecordId,
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


        [HttpGet]
        [Route("page-likes")]
        public IActionResult GetPageLiked(int page, int pageSize)
        {
            try
            {
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest("Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0.");
                }

                int total = dbContext.LogRecord.Count(); // Total number of records
                int totalPages = (int)Math.Ceiling(total / (double)pageSize); // Calculate total pages

                // Calculate the starting index for the page
                int skip = page * pageSize;

                // Get the requested page of items
                var records = dbContext.LogRecord
                    .OrderByDescending(x => x.Likes) // Order by Likes in descending order
                    .Skip(skip) // Skip items for previous pages
                    .Take(pageSize) // Take items for the current page
                    .Select(x => new
                    {
                        RecordId = x.RecordId,
                        path = x.Path,
                        views = x.Views,
                        likes = x.Likes,
                        isNsfw = x.IsNsfw,
                        createdOn = x.CreatedOn
                    })
                    .ToList(); // Convert the result to a list

                // Create the response object
                var result = new
                {
                    Records = records,
                    From = skip + 1, // First item index of this page
                    To = Math.Min(skip + records.Count, total), // Last item index of this page
                    Total = total, // Total number of records
                    Pages = totalPages - 1// Total number of pages
                };

                return Ok(result); // Return the result object
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle any exceptions
            }
        }

        [HttpGet]
        [Route("page-views")]
        public IActionResult GetPageViewed(int page, int pageSize)
        {
            try
            {
                if (page < 0 || pageSize <= 0)
                {
                    return BadRequest("Invalid input: 'page' must be 0 or greater, and 'pageSize' must be greater than 0.");
                }

                int total = dbContext.LogRecord.Count(); // Total number of records
                int totalPages = (int)Math.Ceiling(total / (double)pageSize); // Calculate total pages

                // Calculate the starting index for the page
                int skip = page * pageSize;

                // Get the requested page of items
                var records = dbContext.LogRecord
                    .OrderByDescending(x => x.Views) // Order by Likes in descending order
                    .Skip(skip) // Skip items for previous pages
                    .Take(pageSize) // Take items for the current page
                    .Select(x => new
                    {
                        RecordId = x.RecordId,
                        path = x.Path,
                        views = x.Views,
                        likes = x.Likes,
                        isNsfw = x.IsNsfw,
                        createdOn = x.CreatedOn
                    })
                    .ToList(); // Convert the result to a list

                // Create the response object
                var result = new
                {
                    Records = records,
                    From = skip + 1, // First item index of this page
                    To = Math.Min(skip + records.Count, total), // Last item index of this page
                    Total = total, // Total number of records
                    Pages = totalPages - 1// Total number of pages
                };

                return Ok(result); // Return the result object
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle any exceptions
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
                    RecordId = recordsDomainModel.RecordId,
                    Path = recordsDomainModel.Path,
                    Views = recordsDomainModel.Views,
                    Likes = recordsDomainModel.Likes,
                    IsNsfw = recordsDomainModel.IsNsfw,
                    CreatedOn = recordsToAdd.CreatedOn

                };

                newRecordsDto.CreatedOn = DateTime.Now;

                return CreatedAtAction(nameof(GetById), new { id = newRecordsDto.RecordId }, newRecordsDto);
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
                    RecordId = record.RecordId,
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

        [HttpPost("{id}/add-like")]
        public IActionResult AddLike(int id)
        {
            try
            {
                // Find the record by ID
                var record = dbContext.LogRecord.FirstOrDefault(r => r.RecordId == id);

                if (record == null)
                {
                    return NotFound($"Record with ID {id} not found.");
                }

                // Increment the Likes property
                record.Likes += 1;

                // Save changes to the database
                dbContext.SaveChanges();

                // Return the updated record
                var updatedRecordDto = new RecordsDto
                {
                    RecordId = record.RecordId,
                    Path = record.Path,
                    Views = record.Views,
                    Likes = record.Likes,
                    IsNsfw = record.IsNsfw,
                    CreatedOn = record.CreatedOn
                };

                return Ok(updatedRecordDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("{id}/remove-like")]
        public IActionResult RemoveLike(int id)
        {
            try
            {
                // Find the record by ID
                var record = dbContext.LogRecord.FirstOrDefault(r => r.RecordId == id);

                if (record == null)
                {
                    return NotFound($"Record with ID {id} not found.");
                }

                // Decrement the Likes property, ensuring it doesn't go below zero
                if (record.Likes > 0)
                {
                    record.Likes = record.Likes - 1;
                }

                // Save changes to the database
                dbContext.SaveChanges();

                // Return the updated record
                var updatedRecordDto = new RecordsDto
                {
                    RecordId = record.RecordId,
                    Path = record.Path,
                    Views = record.Views,
                    Likes = record.Likes,
                    IsNsfw = record.IsNsfw,
                    CreatedOn = record.CreatedOn
                };

                return Ok(updatedRecordDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("{id}/add-view")]
        public IActionResult AddView(int id)
        {
            try
            {
                // Find the record by ID
                var record = dbContext.LogRecord.FirstOrDefault(r => r.RecordId == id);

                if (record == null)
                {
                    return NotFound($"Record with ID {id} not found.");
                }

                // Increment the Likes property
                record.Views += 1;

                // Save changes to the database
                dbContext.SaveChanges();

                // Return the updated record
                var updatedRecordDto = new RecordsDto
                {
                    RecordId = record.RecordId,
                    Path = record.Path,
                    Views = record.Views,
                    Likes = record.Likes,
                    IsNsfw = record.IsNsfw,
                    CreatedOn = record.CreatedOn
                };

                return Ok(updatedRecordDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

    }
}