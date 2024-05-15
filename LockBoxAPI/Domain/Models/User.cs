using System.ComponentModel.DataAnnotations;

namespace LockBox.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] MasterPasswordHash { get; set; } = new byte[32];
        public byte[] MasterPasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public string? EmailVerificationCode { get; set; }
        public bool? Verified { get; set; }
    }
}
