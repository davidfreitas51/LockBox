using System.ComponentModel.DataAnnotations;

namespace LockBox.Models
{
    public class RegisteredAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public AppUser User { get; set; }
    }
}
