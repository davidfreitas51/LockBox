﻿using LockBox.Models;
using LockBoxAPI.Repository.Contracts;
using LockBoxAPI.Repository.Database;

public class RegisteredAccountRepository : IRegisteredAccountRepository
{
    private readonly LockBoxContext _context;
    public RegisteredAccountRepository(LockBoxContext context)
    {
        _context = context;
    }

    public void DeleteRegisteredAccount(string RAId)
    {
        var acc = _context.RegisteredAccounts.Where(r => r.Id.ToString() == RAId).FirstOrDefault();
        _context.Remove(acc);
        _context.SaveChanges();
    }

    public List<RegisteredAccount> GetRegisteredAccountsByUser(AppUser user)
    {
        return _context.RegisteredAccounts
               .Where(ra => ra.UserId == user.Id)
               .OrderBy(ra => ra.Title) 
               .ToList();
    }
    public RegisteredAccount GetRegisteredAccountById(string id)
    {
        return _context.RegisteredAccounts
               .Where(ra => ra.Id.ToString() == id)
               .FirstOrDefault();
    }

    public void RegisterAccount(RegisteredAccount accToRegister)
    {
        _context.RegisteredAccounts.Add(accToRegister);
        _context.SaveChanges();
    }

    public void UpdateRegisteredAccount(RegisteredAccount updatedAcc)
    {

        var oldAcc = _context.RegisteredAccounts.Where(acc => acc.Id == updatedAcc.Id).FirstOrDefault();
        if (oldAcc != null)
        {
            oldAcc.Title = updatedAcc.Title;
            oldAcc.Username = updatedAcc.Username;
            oldAcc.Password = updatedAcc.Password;
        }
        _context.SaveChanges();

    }

    public string CopyPassword(string RAId)
    {
        string password = "";
        var userAcc = _context.RegisteredAccounts.Where(acc => acc.Id.ToString() == RAId).FirstOrDefault();
        if (userAcc != null)
        {
            password = userAcc.Password;
        }
        return password;
    }
}
