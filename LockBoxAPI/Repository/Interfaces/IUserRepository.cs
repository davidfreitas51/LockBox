using LockBox.Models;

namespace LockBoxAPI.Repository.Contracts
{
    public interface IUserRepository
    {
        public void CreateUser(User user);
        public User GetByEmail(string email);
        public void UpdateUser(User user);
        public void DeleteUser(User user);
    }
}
