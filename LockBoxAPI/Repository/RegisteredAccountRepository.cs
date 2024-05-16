using LockBox.Models;
using LockBoxAPI.Repository.Contracts;

namespace LockBoxAPI.Repository
{
    public class RegisteredAccountRepository : IRegisteredAccountRepository
    {
        public void CreateAccountPasswords(AppUser user, RegisteredAccount registeredAccount)
        {
            throw new NotImplementedException();
        }

        public void DeleteRegisteredAccount(AppUser user, int id)
        {
            throw new NotImplementedException();
        }

        public List<RegisteredAccount> GetAllAccountPasswords(AppUser user)
        {
            throw new NotImplementedException();
        }

        public void UpdateRegisteredAccount(AppUser user, int id)
        {
            throw new NotImplementedException();
        }
    }
}
