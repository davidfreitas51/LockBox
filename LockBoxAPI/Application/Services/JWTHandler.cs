﻿using LockBox.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LockBoxAPI.Application.Services
{
    public class JWTHandler
    {
        private readonly IConfiguration _configuration;
        public JWTHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateJWT(AppUser appUser)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, appUser.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                 );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        public string DecodeJwt(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokenValidationParams = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                jwtHandler.ValidateToken(token, tokenValidationParams, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var claimsString = "";
                    foreach (var claim in jwtToken.Claims)
                    {
                        claimsString += $"{claim.Type}: {claim.Value}\n";
                    }
                    return claimsString;
                }
                else
                {
                    return "";
                }
            }
            catch (SecurityTokenException)
            {
                return "";
            }
        }
    }
}
