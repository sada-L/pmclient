using System.Collections.Generic;

namespace pmclient.Models;

public sealed class User
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public IEnumerable<Card>? Cards { get; init; }
}