using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pmclient.Helpers;

public static class FileStorage
{
    private static readonly string StoragePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PassManager");

    private static readonly byte[] Key = Encoding.UTF8.GetBytes("your-32-char-long-key-here-12345");
    private static readonly byte[] Iv = Encoding.UTF8.GetBytes("your-16-char-iv-");

    public static async Task SaveFileAsync(string file, string fileName)
    {
        var path = Path.Combine(StoragePath, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        using var encryptor = aes.CreateEncryptor();
        await using var fileStream = new FileStream(path, FileMode.Create);
        await using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write);
        await using var writer = new StreamWriter(cryptoStream);
        await writer.WriteAsync(file);
    }

    public static async Task<string?> LoadFileAsync(string fileName)
    {
        var path = Path.Combine(StoragePath, fileName);
        if (!File.Exists(path)) return null;

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        using var decryptor = aes.CreateDecryptor();
        await using var fileStream = new FileStream(path, FileMode.Open);
        await using var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        return await reader.ReadToEndAsync();
    }

    public static Task DeleteFileAsync(string fileName)
    {
        var path = Path.Combine(StoragePath, fileName);
        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }
}