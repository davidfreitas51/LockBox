﻿using LockBox.Commons.Models;
using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Commons.Services;
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
            var user = _context.Users.Where(u => u.JwtHash == _securityHandler.HashString(request.Token)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }
            
            request.UserAccount.UserId = user.Id;
            _registeredAccountRepository.RegisterAccount(request.UserAccount);
            return Ok();
        }


        [HttpPost("Get")]
        public IActionResult Get(RAGetByUserRequest request)
        {
            var user = _context.Users.Where(u => u.JwtHash == _securityHandler.HashString(request.Token)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }

            List<RegisteredAccount> registeredAccount = _registeredAccountRepository.GetRegisteredAccountsByUser(user); 
            if (registeredAccount == null)
            {
                return NotFound();
            }
            string jsonAccounts = JsonSerializer.Serialize(registeredAccount);
            return Ok(jsonAccounts);
        }


        [HttpPost("GetById")]
        public IActionResult GetById(RAGetByIdRequest request)
        {
            var user = _context.Users.Where(u => u.JwtHash == _securityHandler.HashString(request.Token)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }

            RegisteredAccount registeredAccount = _registeredAccountRepository.GetRegisteredAccountById(request.RAId);
            if (registeredAccount == null)
            {
                return NotFound();
            }
            string jsonAccounts = JsonSerializer.Serialize(registeredAccount);
            return Ok(jsonAccounts);
        }

        [HttpPost("Update")]
        public IActionResult Update(RARequest request)
        {
            var user = _context.Users.Where(u => u.JwtHash == _securityHandler.HashString(request.Token)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }
            _registeredAccountRepository.UpdateRegisteredAccount(request.UserAccount);
            return Ok();
        }


        [HttpPost("DeleteAccount")]
        public IActionResult DeleteAccount(RAGetByIdRequest request)
        {
            var user = _context.Users.Where(u => u.JwtHash == _securityHandler.HashString(request.Token)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }
            _registeredAccountRepository.DeleteRegisteredAccount(request.RAId);
            return Ok();
        }
    }
}
