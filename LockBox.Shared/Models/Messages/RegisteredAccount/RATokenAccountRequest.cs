using LockBox.Models;
namespace LockBox.Commons.Models
{
    public class RATokenAccountRequest
    {
        public string Token { get; set; }
        public RegisteredAccount UserAccount { get; set; }
    }
}
