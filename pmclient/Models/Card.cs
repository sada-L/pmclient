using System.Text.Json.Serialization;

namespace pmclient.Models;

public sealed class Card
{
    [JsonPropertyName("id")] 
    public required int Id { get; init; }

    [JsonPropertyName("group_id")] 
    public required int GroupId { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("username")] 
    public required string Username { get; init; }

    [JsonPropertyName("password")] 
    public required string Password { get; init; }

    [JsonPropertyName("website")] 
    public required string Website { get; init; }

    [JsonPropertyName("notes")]
    public required string Notes { get; init; }

    [JsonPropertyName("image")] 
    public required char Image { get; init; }

    [JsonPropertyName("is_favorite")] 
    public required bool IsFavorite { get; init; }
}