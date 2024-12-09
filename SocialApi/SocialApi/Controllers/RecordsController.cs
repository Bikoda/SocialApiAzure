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
        [Route("page-records")]
        public IActionResult GetPageRecords(int page, int pageSize, string orderBy = "Likes", bool? isNsfw = null)
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
                var recordsQuery = dbContext.LogRecord.AsQueryable();

                // Apply filtering for IsNsfw if provided
                if (isNsfw.HasValue)
                {
                    recordsQuery = recordsQuery.Where(x => x.IsNsfw == isNsfw.Value);
                }

                // Dynamic ordering based on the orderBy parameter
                switch (orderBy)
                {
                    case "Likes":
                        recordsQuery = recordsQuery.OrderByDescending(x => x.Likes);
                        break;

                    case "Views":
                        recordsQuery = recordsQuery.OrderByDescending(x => x.Views);
                        break;

                    case "CreatedOn":
                        recordsQuery = recordsQuery.OrderByDescending(x => x.CreatedOn);
                        break;
                }

                // Total number of filtered records
                int total = recordsQuery.Count();
                int totalPages = (int)Math.Ceiling(total / (double)pageSize); // Calculate total pages

                // Calculate the starting index for the page
                int skip = page * pageSize;

                // Apply pagination and projection
                var records = recordsQuery
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
                    Pages = totalPages - 1 // Total number of pages
                };

                return Ok(result); // Return the result object
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle any exceptions
            }
        }
        /*
        [HttpGet]
        [Route("page-records")]
        public IActionResult GetPageRecords(int page, int pageSize, string orderBy = "Likes")
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

                // Total number of records
                int total = dbContext.LogRecord.Count();
                int totalPages = (int)Math.Ceiling(total / (double)pageSize); // Calculate total pages

                // Calculate the starting index for the page
                int skip = page * pageSize;

                // Base query
                var recordsQuery = dbContext.LogRecord.AsQueryable();

                // Dynamic ordering based on the orderBy parameter
                switch (orderBy)
                {
                    case "Likes":
                        recordsQuery = recordsQuery.OrderByDescending(x => x.Likes);
                        break;

                    case "Views":
                        recordsQuery = recordsQuery.OrderByDescending(x => x.Views);
                        break;

                    case "CreatedOn":
                        recordsQuery = recordsQuery.OrderByDescending(x => x.CreatedOn);
                        break;
                }

                // Apply pagination and projection
                var records = recordsQuery
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
                    Pages = totalPages - 1 // Total number of pages
                };

                return Ok(result); // Return the result object
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle any exceptions
            }
        }*/


        [HttpPost]
        public IActionResult CreateRecord([FromBody] AddRecordsRequestDto recordsToAdd)
        {
            try
            {
                // Parse Views
                if (!long.TryParse(recordsToAdd.Views, out var parsedViews))
                {
                    return BadRequest($"Invalid value for Views: {recordsToAdd.Views}");
                }

                // Parse Likes
                if (!long.TryParse(recordsToAdd.Likes, out var parsedLikes))
                {
                    return BadRequest($"Invalid value for Likes: {recordsToAdd.Likes}");
                }

                // Create the domain model
                var recordsDomainModel = new Records
                {
                    Path = recordsToAdd.Path,
                    Views = parsedViews,
                    Likes = parsedLikes,
                    IsNsfw = recordsToAdd.IsNsfw,
                    CreatedOn = recordsToAdd.CreatedOn
                };

                dbContext.Add(recordsDomainModel);
                dbContext.SaveChanges();

                // Create the DTO for the response
                var newRecordsDto = new RecordsDto
                {
                    RecordId = recordsDomainModel.RecordId,
                    Path = recordsDomainModel.Path,
                    Views = recordsDomainModel.Views,
                    Likes = recordsDomainModel.Likes,
                    IsNsfw = recordsDomainModel.IsNsfw,
                    CreatedOn = DateTime.Now // Use current timestamp for CreatedOn
                };

                return CreatedAtAction(nameof(GetById), new { id = newRecordsDto.RecordId }, newRecordsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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