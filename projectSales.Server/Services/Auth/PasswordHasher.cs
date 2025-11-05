using System.Security.Cryptography;
using System.Text;

namespace projectSales.Server.Services.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password);
}

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}
