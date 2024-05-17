using LockBox.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LockBox.Commons.Services
{
    public class JWTHandler
    {
        private readonly IConfiguration _configuration;
        private readonly string Key = "WqYsL2tA6X1Yo1MlInDmF7bPdGxVHrQcJ4oZvTgKjIeU9yB3lC5fR8wS0MzNpOuYhX6WqYsL2tA6X1nDmF7bPdGxVHrQcJ4oZvTgKjIeU9yB3lC5fR8wS0MzNpOuYhX6";
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
                _configuration.GetSection(Key).Value!));

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
                _configuration.GetSection(Key).Value!)),
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
