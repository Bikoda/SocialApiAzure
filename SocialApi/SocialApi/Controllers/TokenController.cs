using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialApi.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("generate")]
        [AllowAnonymous]
        public IActionResult GenerateToken([FromHeader] string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                return BadRequest("API Key cannot be null or empty.");

            try
            {
                if (apiKey != _configuration["Jwt:Key"])
                    return Unauthorized("Invalid API Key.");

                // Generate a simple JWT for demonstration purposes
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, "user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds);

                var tokenResponse = new TokenResponseDto(new JwtSecurityTokenHandler().WriteToken(token));
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}