using LockBox.Models;

namespace LockBox.Commons.Models.Messages
{
    public class JwtResponse
    {
        public string Token { get; set; }
        public AppUser AppUser { get; set; }
    }

}
