using LockBox.Commons.Models;
using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Commons.Services.Interfaces;
using LockBox.Models;
using LockBoxAPI.Repository.Contracts;
using LockBoxAPI.Repository.Database;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        public readonly IRegisteredAccountRepository _registeredAccountRepository;
        public readonly ISecurityHandler _securityHandler;
        public readonly LockBoxContext _context;
        public AccountsController(IRegisteredAccountRepository registeredAccountRepository, ISecurityHandler securityHandler, LockBoxContext context)
        {
            _registeredAccountRepository = registeredAccountRepository;
            _securityHandler = securityHandler;
            _context = context;
        }


        [HttpPost("Register")]
        public IActionResult Register(RATokenAccountRequest request)
        {
            var user = AuthenticateUser(request.Token);
            if (user == null)
            {
                return BadRequest();
            }
            
            request.UserAccount.UserId = user.Id;
            request.UserAccount.Password = _securityHandler.EncryptAES(request.UserAccount.Password);
            _registeredAccountRepository.RegisterAccount(request.UserAccount);
            return Ok();
        }


        [HttpPost("Get")]
        public IActionResult Get(RATokenRequest request)
        {
            var user = AuthenticateUser(request.Token);
            if (user == null)
            {
                return BadRequest();
            }

            var registeredAccount = _registeredAccountRepository.GetRegisteredAccountsByUser(user); 
            if (registeredAccount == null)
            {
                return NotFound();
            }
            foreach (var acc in registeredAccount)
            {
                acc.Password = _securityHandler.DecryptAES(acc.Password);
            }
            var jsonAccounts = JsonSerializer.Serialize(registeredAccount);
            return Ok(jsonAccounts);
        }


        [HttpPost("GetById")]
        public IActionResult GetById(RATokenAccIdRequest request)
        {
            var user = AuthenticateUser(request.Token);
            if (user == null)
            {
                return BadRequest();
            }

            var registeredAccount = _registeredAccountRepository.GetRegisteredAccountById(request.RAId);
            if (registeredAccount == null)
            {
                return NotFound();
            }
            registeredAccount.Password = _securityHandler.DecryptAES(registeredAccount.Password);
            var jsonAccounts = JsonSerializer.Serialize(registeredAccount);
            return Ok(jsonAccounts);
        }

        [HttpPost("Update")]
        public IActionResult Update(RATokenAccountRequest request)
        {
            var user = AuthenticateUser(request.Token);
            if (user == null)
            {
                return BadRequest();
            }
            request.UserAccount.Password = _securityHandler.EncryptAES(request.UserAccount.Password);
            _registeredAccountRepository.UpdateRegisteredAccount(request.UserAccount);
            return Ok();
        }


        [HttpPost("DeleteAccount")]
        public IActionResult DeleteAccount(RATokenAccIdRequest request)
        {
            var user = AuthenticateUser(request.Token);
            if (user == null)
            {
                return BadRequest();
            }
            _registeredAccountRepository.DeleteRegisteredAccount(request.RAId);
            return Ok();
        }


        [HttpPost("CopyPassword")]
        public IActionResult CopyPassword(RATokenAccIdRequest request)
        {
            var user = AuthenticateUser(request.Token);
            if (user == null)
            {
                return BadRequest();
            }
            var passwordAES = _registeredAccountRepository.CopyPassword(request.RAId);
            var password = _securityHandler.DecryptAES(passwordAES);
            return Ok(password);
        }
        private AppUser AuthenticateUser(string token)
        {
            return _context.Users.FirstOrDefault(u => u.JwtHash == _securityHandler.HashString(token));
        }
    }
}
