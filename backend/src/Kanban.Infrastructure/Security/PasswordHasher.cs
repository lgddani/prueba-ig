using System.Security.Cryptography;
using Kanban.Application.Abstractions.Security;
using Microsoft.Extensions.Options;

namespace Kanban.Infrastructure.Security;

/// <summary>
/// PBKDF2-HMACSHA256 con salt aleatorio por usuario (almacenado junto al hash) y
/// pepper de servidor
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSizeBytes = 16;
    private const int KeySizeBytes = 32;
    private const int Iterations = 100_000;

    private readonly string _pepper;

    public PasswordHasher(IOptions<PasswordHasherOptions> options) => _pepper = options.Value.Pepper;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var key = Rfc2898DeriveBytes.Pbkdf2(password + _pepper, salt, Iterations, HashAlgorithmName.SHA256, KeySizeBytes);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);
        var testKey = Rfc2898DeriveBytes.Pbkdf2(password + _pepper, salt, iterations, HashAlgorithmName.SHA256, key.Length);

        return CryptographicOperations.FixedTimeEquals(key, testKey);
    }
}
