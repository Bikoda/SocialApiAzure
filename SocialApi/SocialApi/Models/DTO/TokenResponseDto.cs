﻿namespace SocialApi.Models.DTO
{
    public class TokenResponseDto
    {
        public string Token { get; set; }

        public TokenResponseDto(string token)
        {
            Token = token;
        }
    }
}