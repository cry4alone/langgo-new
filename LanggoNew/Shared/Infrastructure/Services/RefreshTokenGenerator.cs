using System.Security.Cryptography;
using System.Text;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface IRefreshTokenGenerator
{
    public string GenerateRefreshToken();
    public string HashToken(string token);
}

public class RefreshTokenGenerator : IRefreshTokenGenerator
{    
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=') 
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public string HashToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }
    
}