using LockBox.Commons.Models.Messages;
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, appUser.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }


        public JwtResponse DecodeJwt(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Key);
            var tokenValidationParams = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
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
                    return new JwtResponse { Claims = claimsString };
                }
                else
                {
                    return new JwtResponse { Error = "Token inválido." };
                }
            }
            catch (SecurityTokenException ex)
            {
                return new JwtResponse { Error = ex.Message };
            }
        }
    }
}