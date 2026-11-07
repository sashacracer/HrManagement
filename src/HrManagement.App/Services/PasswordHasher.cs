using System.Security.Cryptography;

namespace HrManagement.App.Services;

public static class PasswordHasher
{
    private const int Iterations = 120_000;
    private const int KeySize = 32;

    public static (string Hash, string Salt) Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool Verify(string password, string expectedHash, string salt)
    {
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Convert.FromBase64String(salt),
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            Convert.FromBase64String(expectedHash));
    }
}
