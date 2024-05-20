﻿using LockBox.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LockBox.Commons.Services
{
    public class SecurityHandler
    {
        private readonly string Key = "WqYsL2tA6X1Yo1MlInDmF7bPdGxVHrQcJ4oZvTgKjIeU9yB3lC5fR8wS0MzNpOuYhX6WqYsL2tA6X1nDmF7bPdGxVHrQcJ4oZvTgKjIeU9yB3lC5fR8wS0MzNpOuYhX6";
        public string CreateToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        public string HashString(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public bool CompareHash(string plainText, string hashedText)
        {
            string hashedPlainText = HashString(plainText);

            return StringComparer.OrdinalIgnoreCase.Compare(hashedPlainText, hashedText) == 0;
        }
    }
}
