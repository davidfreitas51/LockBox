using LockBox.Models;
using LockBox.Models.Messages;
using LockBoxAPI.Application.Services;
using LockBoxAPI.Repository.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LockBoxContext _context;
        private readonly JWTHandler _jwtHandler;
        private readonly VerificationEmailService _emailVerification;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(LockBoxContext context, VerificationEmailService emailVerification, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JWTHandler jwtHandler)
        {
            _context = context;
            _emailVerification = emailVerification;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtHandler = jwtHandler;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.Email,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                existingUser = await _userManager.FindByEmailAsync(request.Email);
                existingUser.EmailVerificationCode = _emailVerification.VerificationEmail(request.Email);
                _context.SaveChanges();
                return Ok("User successfully created!");
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                ErrorResponse errorResponse = new ErrorResponse
                {
                    Errors = errors
                };
                return BadRequest(errorResponse);

            }
        }


        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyCode(UserEmailVerificationRequest request)
        {
            var user = _context.Users.Where(u => u.Email == request.Email).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            if (user.EmailVerificationCode == request.Code)
            {
                user.EmailConfirmed = true;
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = _context.Users.Where(u => u.Email == request.Email).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            if (user.EmailConfirmed == false)
            {
                return Unauthorized();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (signInResult.Succeeded)
            {
                string token = _jwtHandler.CreateJWT(user);
                return Ok(token);
            }
            if (signInResult.IsLockedOut)
            {
                return StatusCode(429, "Account locked out. Please try again later.");
            }
            return BadRequest("Invalid login attempt.");
        }
    }
}
