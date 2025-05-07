using System.Text.Json.Serialization;

namespace pmclient.Contracts.Response.Auth;

public sealed class AuthResponse
{
    [JsonPropertyName("access_token")] public required string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")] public required string RefreshToken { get; init; }
}