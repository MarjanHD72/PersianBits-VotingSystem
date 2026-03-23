using System.Security.Cryptography;

namespace VotingSystem.Domain;

public static class SessionIdGenerator
{
    private static readonly char[] Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

    public static string Generate()
    {
        // Format: XXXX-XXXX
        Span<char> result = stackalloc char[9];
        for (int i = 0; i < 9; i++)
        {
            if (i == 4) { result[i] = '-'; continue; }
            result[i] = Chars[RandomNumberGenerator.GetInt32(Chars.Length)];
        }
        return new string(result);
    }
}
