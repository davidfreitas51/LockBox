using LockBox.Models;
using LockBoxAPI.Repository.Contracts;

namespace LockBoxAPI.Repository
{
    public class RegisteredAccountRepository : IRegisteredAccountRepository
    {
        public void CreateAccountPasswords(User user, RegisteredAccount registeredAccount)
        {
            throw new NotImplementedException();
        }

        public void DeleteRegisteredAccount(User user, int id)
        {
            throw new NotImplementedException();
        }

        public List<RegisteredAccount> GetAllAccountPasswords(User user)
        {
            throw new NotImplementedException();
        }

        public void UpdateRegisteredAccount(User user, int id)
        {
            throw new NotImplementedException();
        }
    }
}
