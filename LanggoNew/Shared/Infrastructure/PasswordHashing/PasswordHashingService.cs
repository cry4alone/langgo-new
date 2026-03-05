using System.Security.Cryptography;

namespace LanggoNew.Shared.Infrastructure.PasswordHashing;

public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}

public class PasswordHashingService : IPasswordHashingService
{
    private const int SaltSize = 128 / 8;
    private const int HashSize = 256 / 8;
    private const int Iterations = 1000;
        
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
    
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        
        return Convert.ToBase64String(hash) + "-" + Convert.ToBase64String(salt);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword)) return false;

        var parts = hashedPassword.Split('-');
        if (parts.Length != 2) return false;

        byte[] storedHash;
        byte[] salt;
        try
        {
            storedHash = Convert.FromBase64String(parts[0]);
            salt = Convert.FromBase64String(parts[1]);
        }
        catch
        {
            return false;
        }

        var derivedHash = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(storedHash, derivedHash);
    }
}