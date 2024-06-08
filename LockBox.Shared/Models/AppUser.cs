using Microsoft.AspNetCore.Identity;

namespace LockBox.Models
{
    public class AppUser : IdentityUser
    {
        public string? EmailVerificationCode { get; set; }
        public string? JwtHash { get; set; }
    }
}
