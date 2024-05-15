using LockBox.Models;
using Microsoft.EntityFrameworkCore;

namespace LockBoxAPI.Repository.Database
{
    public class LockBoxContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RegisteredAccount> RegisteredAccounts { get; set; }

        public LockBoxContext(DbContextOptions<LockBoxContext> options) : base(options)
        {

        }
    }
}
