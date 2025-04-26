using System.Text.Json.Serialization;

namespace pmclient.Contracts.Requests.Auth;

public sealed class RefreshRequest
{
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }
}