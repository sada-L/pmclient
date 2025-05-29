using pmclient.Models;

namespace pmclient.Settings;

internal static class UserSettings
{
    public static string? JwtToken { get; set; }

    public static User? User { get; set; }

    public static string? Pin { get; set; }
}