using LockBox.Models;

namespace LockBoxAPI.Repository.Contracts
{
    public interface IUserRepository
    {
        public void CreateUser(AppUser user);
        public AppUser GetByEmail(string email);
        public void UpdateUser(AppUser user);
        public void DeleteUser(AppUser user);
    }
}
