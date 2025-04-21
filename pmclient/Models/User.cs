using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pmclient.Models;

public sealed class User
{
    [JsonPropertyName("id")] 
    public required int Id { get; init; }
    [JsonPropertyName("username")]
    public required string Username { get; init; }
    [JsonPropertyName("email")] 
    public required string Email { get; init; }
}