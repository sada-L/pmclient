using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pmclient.Helpers;

public static class TokenStorage
{
    private static readonly string StoragePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PassManager", "token.dat" );

    private static readonly byte[] Key = Encoding.UTF8.GetBytes("your-32-char-long-key-here-12345"); 

    public static async Task SaveTokenAsync(string token)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StoragePath)!);

        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = new byte[16]; 

        using var encryptor = aes.CreateEncryptor();
        await using var fileStream = new FileStream(StoragePath, FileMode.Create);
        await using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write);
        await using var writer = new StreamWriter(cryptoStream);
        await writer.WriteAsync(token);
    }

    public static async Task<string?> LoadTokenAsync()
    {
        if (!File.Exists(StoragePath)) return null;

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = new byte[16];

        using var decryptor = aes.CreateDecryptor();
        await using var fileStream = new FileStream(StoragePath, FileMode.Open);
        await using var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        return await reader.ReadToEndAsync();
    }

    public static Task DeleteTokenAsync()
    {
        if (File.Exists(StoragePath))
            File.Delete(StoragePath);
        
        return Task.CompletedTask;
    }
}