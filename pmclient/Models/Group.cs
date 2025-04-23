using System.Collections.Generic;

namespace pmclient.Models;

public class Group
{
   public required int Id { get; init; }
   public required string Title { get; init; }
   public required string Image { get; init; }
   public required int? GroupId { get; init; }
}