using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pmclient.Models;

public class Group
{
   [JsonPropertyName("id")] 
   public required int Id { get; init; }
   
   [JsonPropertyName("group_id")] 
   public required int GroupId { get; init; }
   
   [JsonPropertyName("title")] 
   public required string Title { get; init; }
   
   [JsonPropertyName("image")] 
   public required string Image { get; init; }
}