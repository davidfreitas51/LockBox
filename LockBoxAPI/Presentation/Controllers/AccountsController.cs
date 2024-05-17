﻿using LockBox.Commons.Models;
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
    public class AccountsController : ControllerBase
    {
        public readonly IRegisteredAccountRepository _registeredAccountRepository;
        public AccountsController(IRegisteredAccountRepository registeredAccountRepository)
        {
            _registeredAccountRepository = registeredAccountRepository;
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
            AppUser user = request.AppUser;
            List<RegisteredAccount> registeredAccount = _registeredAccountRepository.GetRegisteredAccountsByUser(user);

            if (registeredAccount == null)
            {
                return NotFound();
            }
            return Ok(registeredAccount);
        }


        [HttpPost("Update")]
        public IActionResult Update(RARequest request)
        {
            _registeredAccountRepository.UpdateRegisteredAccount(request.UserAccount);
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
