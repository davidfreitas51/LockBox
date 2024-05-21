using LockBox.Models;

namespace LockBox.Commons.Services.Interfaces
{
    public interface ISecurityHandler
    {
        string CreateToken(AppUser user);
        string HashString(string input);
        bool CompareHash(string plainText, string hashedText);
        string EncryptAES(string plainText);
        string DecryptAES(string cipherText);
    }
}
