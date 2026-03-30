using System.Security.Cryptography;
using System.Text;

namespace VotingSystem.Domain;

public static class BCryptHelper
{
    // Simple hash + salt using HMACSHA256 — good enough for a coursework prototype.
    // Format: salt:hash (both base64)

    public static string Hash(string password)
    {
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        var hash = ComputeHash(password, salt);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        var actualHash = ComputeHash(password, salt);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    private static byte[] ComputeHash(string password, byte[] salt)
    {
        using var hmac = new HMACSHA256(salt);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}
