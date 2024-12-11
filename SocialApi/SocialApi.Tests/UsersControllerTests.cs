using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Controllers;
using SocialApi.Data;
using SocialApi.Models.Domain;
using SocialApi.Models.DTO;

namespace SocialApi.Tests
{
    public class UsersControllerTests
    {
        private WebSocialDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WebSocialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
                .Options;

            return new WebSocialDbContext(options);
        }

        [Fact]
        public void GetAllUsers_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Users.AddRange(new List<Users>
            {
                new Users { UserId = 1, Nickname = "User1", Email = "user1@example.com", UserAddress = "Address1", CreatedOn = DateTime.Now },
                new Users { UserId = 2, Nickname = "User2", Email = "user2@example.com", UserAddress = "Address2", CreatedOn = DateTime.Now }
            });
            context.SaveChanges();

            var controller = new UsersController(context);

            // Act
            var result = controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsType<List<UsersDto>>(okResult.Value);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void GetUserById_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Users.Add(new Users
            {
                UserId = 1,
                Nickname = "TestUser",
                Email = "test@example.com",
                UserAddress = "TestAddress",
                CreatedOn = DateTime.UtcNow
            });
            context.SaveChanges();

            var controller = new UsersController(context);

            // Act
            var result = controller.GetUserById("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userDto = Assert.IsType<UsersDto>(okResult.Value);
            Assert.Equal(1, userDto.UserId);
            Assert.Equal("TestUser", userDto.Nickname);
        }

        [Fact]
        public void GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new UsersController(context);

            // Act
            var result = controller.GetUserById("999");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetUserByAddress_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            context.Users.Add(new Users
            {
                UserId = 1,
                Nickname = "TestUser",
                Email = "test@example.com",
                UserAddress = "TestAddress",
                CreatedOn = DateTime.UtcNow
            });
            context.SaveChanges();

            var controller = new UsersController(context);

            // Act
            var result = controller.GetUserByAddress("TestAddress");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userDto = Assert.IsType<UsersDto>(okResult.Value);
            Assert.Equal("TestAddress", userDto.UserAddress);
        }

        [Fact]
        public void GetUserByAddress_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new UsersController(context);

            // Act
            var result = controller.GetUserByAddress("NonExistentAddress");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void CreateUser_ReturnsCreatedAtActionResult_WithNewUser()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new UsersController(context);

            var newUserRequest = new AddUsersRequestDto
            {
                Nickname = "NewUser",
                Email = "newuser@example.com",
                UserAddress = "NewAddress"
            };

            // Act
            var result = controller.CreateUser(newUserRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var userDto = Assert.IsType<UsersDto>(createdAtActionResult.Value);

            Assert.Equal("NewUser", userDto.Nickname);
            Assert.Equal("newuser@example.com", userDto.Email);
            Assert.Equal("NewAddress", userDto.UserAddress);
        }

        [Fact]
        public void CreateUser_Returns500Error_WhenExceptionOccurs()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var controller = new UsersController(context);

            var newUserRequest = new AddUsersRequestDto
            {
                Nickname = "NewUser",
                Email = "newuser@example.com",
                UserAddress = "NewAddress"
            };

            // Simulate an exception by disposing the context
            context.Dispose();

            // Act
            var result = controller.CreateUser(newUserRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}