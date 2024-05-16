using LockBox.Models;

namespace LockBoxAPI.Repository.Contracts
{
    public interface IRegisteredAccountRepository
    {
        public void CreateAccountPasswords(AppUser user, RegisteredAccount registeredAccount);
        public List<RegisteredAccount> GetAllAccountPasswords(AppUser user);
        public void UpdateRegisteredAccount(AppUser user, int id);
        public void DeleteRegisteredAccount(AppUser user, int id);
    }
}
