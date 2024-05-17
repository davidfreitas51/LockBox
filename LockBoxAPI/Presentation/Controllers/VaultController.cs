using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Models;
using LockBoxAPI.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaultController : ControllerBase
    {
        public readonly IRegisteredAccountRepository _registeredAccountRepository;
        public VaultController(IRegisteredAccountRepository registeredAccountRepository)
        {
            _registeredAccountRepository = registeredAccountRepository;
        }

        [HttpGet("GetAccounts")]
        public IActionResult GetAccounts(RAGetByUserRequest request)
        {
            AppUser user = request.AppUser;
            List<RegisteredAccount> registeredAccount = _registeredAccountRepository.GetRegisteredAccountsByUser(user);
            return Ok(registeredAccount);
        }
    }
}
