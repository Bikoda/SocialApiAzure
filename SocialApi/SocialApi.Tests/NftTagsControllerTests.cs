using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static SocialApi.Models.DTO.NftTagsDto;

namespace SocialApi.Tests
{
    public class NftTagsControllerTests
    {
        private WebSocialDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WebSocialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            return new WebSocialDbContext(options);
        }

        [Fact]
        public async void AddTagsToNft_ReturnsOkResult_WithAddedTags()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.Add(new Nfts { NftId = 1, NftAddress = "0x123" });
            context.Tags.Add(new Tags { TagId = 1, Name = "Art" });
            context.SaveChanges();

            var controller = new NftTagsController(context);
            var request = new AddNftTagRequestDto
            {
                NftId = 1,
                TagIds = new List<long> { 1 }
            };

            // Act
            var result = await controller.AddTagsToNft(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(context.NftTags.FirstOrDefault(nt => nt.NftId == 1 && nt.TagId == 1));
        }

        [Fact]
        public async void AddTagsToNft_ReturnsNotFound_WhenNftDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Tags.Add(new Tags { TagId = 1, Name = "Art" });
            context.SaveChanges();

            var controller = new NftTagsController(context);
            var request = new AddNftTagRequestDto
            {
                NftId = 99, // Non-existent NFT
                TagIds = new List<long> { 1 }
            };

            // Act
            var result = await controller.AddTagsToNft(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("NFT not found.", badRequestResult.Value);
        }

        [Fact]
        public async void RemoveTagFromNft_ReturnsOkResult_WhenTagRemovedSuccessfully()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.Add(new Nfts { NftId = 1, NftAddress = "0x123" });
            context.Tags.Add(new Tags { TagId = 1, Name = "Art" });
            context.NftTags.Add(new NftTags { NftId = 1, TagId = 1, CreatedOn = DateTime.UtcNow });
            context.SaveChanges();

            var controller = new NftTagsController(context);

            // Act
            var result = await controller.RemoveTagFromNft(1, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(context.NftTags.FirstOrDefault(nt => nt.NftId == 1 && nt.TagId == 1));
        }

        [Fact]
        public async void RemoveTagFromNft_ReturnsNotFound_WhenTagNotAssociatedWithNft()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Nfts.Add(new Nfts { NftId = 1, NftAddress = "0x123" });
            context.Tags.Add(new Tags { TagId = 1, Name = "Art" });
            context.SaveChanges();

            var controller = new NftTagsController(context);

            // Act
            var result = await controller.RemoveTagFromNft(1, 99); // Non-existent TagId

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value); // Expect a string
            Assert.Equal("Tag not associated with the specified NFT.", message);
        }


        [Fact]
        public void GetNftTags_ReturnsPaginatedResults()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.NftTags.AddRange(
                new NftTags { NftId = 1, TagId = 1, CreatedOn = DateTime.UtcNow },
                new NftTags { NftId = 1, TagId = 2, CreatedOn = DateTime.UtcNow },
                new NftTags { NftId = 2, TagId = 1, CreatedOn = DateTime.UtcNow }
            );
            context.SaveChanges();

            var controller = new NftTagsController(context);

            // Act
            var result = controller.GetNftTags(0, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedNftTagsResponse>(okResult.Value);

            // Validate pagination metadata
            Assert.Equal(1, response.From);
            Assert.Equal(2, response.To);
            Assert.Equal(3, response.Total);
            Assert.Equal(2, response.TotalPages);

            // Validate NFT tags
            Assert.Equal(2, response.NftTags.Count);
            Assert.Contains(response.NftTags, nt => nt.NftId == 1 && nt.TagId == 1);
            Assert.Contains(response.NftTags, nt => nt.NftId == 1 && nt.TagId == 2);
        }

        [Fact]
        public void GetNftTags_FiltersByNftId()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.NftTags.AddRange(
                new NftTags { NftId = 1, TagId = 1, CreatedOn = DateTime.UtcNow },
                new NftTags { NftId = 1, TagId = 2, CreatedOn = DateTime.UtcNow },
                new NftTags { NftId = 2, TagId = 1, CreatedOn = DateTime.UtcNow }
            );
            context.SaveChanges();

            var controller = new NftTagsController(context);

            // Act
            var result = controller.GetNftTags(0, 5, nftId: 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedNftTagsResponse>(okResult.Value);

            Assert.Equal(2, response.NftTags.Count);
            Assert.All(response.NftTags, nt => Assert.Equal(1L, nt.NftId));
        }



    }
}