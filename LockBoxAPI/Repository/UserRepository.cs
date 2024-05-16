using LockBox.Models;
using LockBoxAPI.Repository.Contracts;

namespace LockBoxAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        public void CreateUser(AppUser user)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(AppUser user)
        {
            throw new NotImplementedException();
        }

        public AppUser GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
