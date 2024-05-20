using LockBox.Commons.Models;
using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Commons.Services;
using LockBox.Models;
using LockBoxAPI.Repository.Contracts;
using LockBoxAPI.Repository.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        public readonly IRegisteredAccountRepository _registeredAccountRepository;
        public readonly SecurityHandler _securityHandler;
        public readonly LockBoxContext _context;
        public AccountsController(IRegisteredAccountRepository registeredAccountRepository, SecurityHandler securityHandler, LockBoxContext context)
        {
            _registeredAccountRepository = registeredAccountRepository;
            _securityHandler = securityHandler;
            _context = context;
        }


        [HttpPost("Register")]
        public IActionResult Register(RARequest request)
        {
            _registeredAccountRepository.RegisterAccount(request.UserAccount);
            return Ok();
        }


        [HttpPost("Get")]
        public IActionResult Get(RAGetByUserRequest request)
        {
            var user = _context.Users.Where(u => u.Email == request.AppUser.Email).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }
            if (!_securityHandler.CompareHash(request.Token, user.JwtHash))
            {
                return Forbid();
            }

            List<RegisteredAccount> registeredAccount = _registeredAccountRepository.GetRegisteredAccountsByUser(user); 
            if (registeredAccount == null)
            {
                return NotFound();
            }
            return Ok(registeredAccount);
        }


        [HttpPost("Update")]
        public IActionResult Update()
        {
            return Ok();
        }


        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteAccount(RARequest request)
        {
            _registeredAccountRepository.RegisterAccount(request.UserAccount);
            return Ok();
        }
    }
}
