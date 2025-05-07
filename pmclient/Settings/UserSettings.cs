using pmclient.Models;

namespace pmclient.Services;

internal static class UserSettings
{
    public static string? JwtToken { get; set; }

    public static User? User { get; set; }
}