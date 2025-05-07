using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pmclient.Helpers;

public static class PasswordGenerator
{
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

    public static string GenerateSecurePassword(
        int length = 16,
        bool requireDigit = true,
        bool requireLowercase = true,
        bool requireUppercase = true,
        bool requireSpecialChar = true)
    {
        if (length < 8)
            throw new ArgumentException("Длина пароля должна быть не менее 8 символов");

        var charPool = new StringBuilder();
        var password = new char[length];

        if (requireLowercase) charPool.Append(LowerCase);
        if (requireUppercase) charPool.Append(UpperCase);
        if (requireDigit) charPool.Append(Digits);
        if (requireSpecialChar) charPool.Append(SpecialChars);

        if (charPool.Length == 0)
            throw new ArgumentException("Должен быть включен хотя бы один набор символов");

        using (var rng = RandomNumberGenerator.Create())
        {
            for (var i = 0; i < length; i++)
            {
                password[i] = GetRandomChar(charPool.ToString(), rng);
            }

            if (requireLowercase && !ContainsAny(password, LowerCase))
                password[GetRandomIndex(rng, length)] = GetRandomChar(LowerCase, rng);

            if (requireUppercase && !ContainsAny(password, UpperCase))
                password[GetRandomIndex(rng, length)] = GetRandomChar(UpperCase, rng);

            if (requireDigit && !ContainsAny(password, Digits))
                password[GetRandomIndex(rng, length)] = GetRandomChar(Digits, rng);

            if (requireSpecialChar && !ContainsAny(password, SpecialChars))
                password[GetRandomIndex(rng, length)] = GetRandomChar(SpecialChars, rng);
        }

        return new string(password);
    }

    private static char GetRandomChar(string charPool, RandomNumberGenerator rng)
    {
        var randomByte = new byte[1];
        rng.GetBytes(randomByte);
        return charPool[randomByte[0] % charPool.Length];
    }

    private static int GetRandomIndex(RandomNumberGenerator rng, int max)
    {
        var randomByte = new byte[1];
        rng.GetBytes(randomByte);
        return randomByte[0] % max;
    }

    private static bool ContainsAny(char[] password, string charSet)
    {
        return password.Any(charSet.Contains);
    }
}