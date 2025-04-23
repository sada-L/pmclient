namespace pmclient.Models;

public sealed class Card
{
   public required int Id { get; init; }
   public required string Title { get; init; }
   public required string Username { get; init; }
   public required string Url { get; init; }
   public required string Image { get; init; }
   public required string Notes { get; init; }
   public required string Password { get; init; }
   public required int? GroupId { get; init; }
   public required bool IsFavorite { get; init; }
}