using LockBox.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LockBoxAPI.Repository.Database
{
    public class LockBoxContext : IdentityDbContext<AppUser>
    {
        public DbSet<RegisteredAccount> RegisteredAccounts { get; set; }

        public LockBoxContext(DbContextOptions<LockBoxContext> options) : base(options)
        {

        }
    }
}
