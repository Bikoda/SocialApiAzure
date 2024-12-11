using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SocialApi.Controllers;
using SocialApi.Models.DTO;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Xunit;

namespace SocialApi.Tests
{
    public class TokenControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new TokenController(_mockConfiguration.Object);
        }

        [Fact]
        public void GenerateToken_ReturnsOkResult_WithToken_WhenApiKeyIsValid()
        {
            // Arrange
            var validApiKey = "thisisaverylongapikeythatshouldbe32chars"; // 32 characters (256 bits)
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns(validApiKey);
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("testIssuer");
            _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("testAudience");

            // Act
            var result = _controller.GenerateToken(validApiKey);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenResponse = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.NotNull(tokenResponse.Token);  // Ensure token is returned

            // Verify if the returned token is a valid JWT format
            var tokenParts = tokenResponse.Token.Split('.');
            Assert.Equal(3, tokenParts.Length); // JWT should have 3 parts: Header, Payload, Signature
        }

        [Fact]
        public void GenerateToken_ReturnsUnauthorized_WhenApiKeyIsInvalid()
        {
            // Arrange
            var invalidApiKey = "invalid-api-key";
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("valid-api-key");

            // Act
            var result = _controller.GenerateToken(invalidApiKey);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid API Key.", unauthorizedResult.Value);
        }

        [Fact]
        public void GenerateToken_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var apiKey = "valid-api-key";
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Throws(new Exception("Some error"));

            // Act
            var result = _controller.GenerateToken(apiKey);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Error: Some error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public void GenerateToken_ReturnsBadRequest_WhenApiKeyIsEmpty()
        {
            // Arrange
            var emptyApiKey = string.Empty;
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("valid-api-key");

            // Act
            var result = _controller.GenerateToken(emptyApiKey);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("API Key cannot be null or empty.", badRequestResult.Value);
        }
    }
}