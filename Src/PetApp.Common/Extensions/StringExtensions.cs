using System.Security.Cryptography;
using System.Text;

namespace PetApp.Common.Extensions;

public static class StringExtensions
{
    public static string GetSha256Hash(this string input)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash);
    }
}