using LockBox.Models;
using LockBox.Models.Messages;
using LockBoxAPI.Application.Services;
using LockBoxAPI.Repository.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LockBoxContext _context;
        private readonly VerificationEmailService _emailVerification;
        public UserController(LockBoxContext context, VerificationEmailService emailVerification)
        {
            _context = context;
            _emailVerification = emailVerification;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User already exists");
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                MasterPasswordHash = passwordHash,
                MasterPasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken(),
                Verified = false,
                EmailVerificationCode = _emailVerification.VerificationEmail(request.Email)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User successfully created!");
        }


        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyCode(UserVerificationEmailRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == request.Token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.EmailVerificationCode != request.Code)
            {
                return BadRequest("The code don't match");
            }

            user.Verified = true;
            await _context.SaveChangesAsync();

            return Ok("User verified!");
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
