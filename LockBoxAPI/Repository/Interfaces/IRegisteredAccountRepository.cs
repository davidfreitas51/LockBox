using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Models;

namespace LockBoxAPI.Repository.Contracts
{
    public interface IRegisteredAccountRepository
    {
        public void RegisterAccount(RegisteredAccount accToRegister);
        public List<RegisteredAccount> GetRegisteredAccountsByUser(AppUser user);
        public void UpdateRegisteredAccount(RegisteredAccount accToUpdate);
        public void DeleteRegisteredAccount(RegisteredAccount accToDelete);
    }
}
