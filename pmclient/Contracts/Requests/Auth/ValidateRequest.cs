using System.Text.Json.Serialization;

namespace pmclient.Contracts.Requests.Auth;

public sealed class ValidateRequest
{
    [JsonPropertyName("code")] public required string Code { get; init; }
}