using LockBox.Models;

namespace LockBoxAPI.Repository.Contracts
{
    public interface IRegisteredAccountRepository
    {
        public void CreateAccountPasswords(User user, RegisteredAccount registeredAccount);
        public List<RegisteredAccount> GetAllAccountPasswords(User user);
        public void UpdateRegisteredAccount(User user, int id);
        public void DeleteRegisteredAccount(User user, int id);
    }
}
