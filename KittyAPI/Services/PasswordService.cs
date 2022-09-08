using System.Security.Cryptography;
using System.Text;

namespace KittyAPI.Services;

public static class PasswordService
{
    public static void ComputeHashSHA512(string text, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
            
        }
    }

    public static bool VerifyHashSHA512(string text, byte[] passwordHash, byte[] passwordSalt)
    {
        HMACSHA512 hmac = new HMACSHA512(passwordSalt);
        var inc = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
        return passwordHash.SequenceEqual(inc);
        
    }
}
