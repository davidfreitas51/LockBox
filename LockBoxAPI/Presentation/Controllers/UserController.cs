using LockBox.Commons.Models.Messages;
using LockBox.Commons.Models.Messages.User;
using LockBox.Commons.Services.Interfaces;
using LockBox.Models;
using LockBoxAPI.Repository.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LockBoxContext _context;
        private readonly ISecurityHandler _securityHandler;
        private readonly IVerificationEmailService _emailVerification;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(LockBoxContext context, IVerificationEmailService emailVerification, ISecurityHandler securityHandler, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _emailVerification = emailVerification;
            _userManager = userManager;
            _signInManager = signInManager;
            _securityHandler = securityHandler;
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
                return Ok("User successfully created!");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            ErrorResponse errorResponse = new ErrorResponse();
            errorResponse.Errors = errors;

            return BadRequest(errorResponse);
        }


        [HttpPost("SendVerificationCode")]
        public async Task<IActionResult> SendVerificationCode(UserAskConfirmationEmail request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if ( user == null)
            {
                return NotFound();
            }

            user.EmailVerificationCode = _emailVerification.VerificationEmail(request.Email);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyCode(UserEmailVerificationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
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
            var user = await _userManager.FindByEmailAsync(request.Email);

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
                string jwtToken = _securityHandler.CreateToken(user);
                string hashedJwt = _securityHandler.HashString(jwtToken);

                user.JwtHash = hashedJwt;
                _context.SaveChanges();

                return Ok(jwtToken);
            }
            if (signInResult.IsLockedOut)
            {
                return StatusCode(429, "Account locked out. Please try again later.");
            }
            return BadRequest("Invalid login attempt.");
        }

        
    }
}