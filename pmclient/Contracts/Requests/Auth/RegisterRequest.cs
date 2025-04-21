namespace pmclient.Contracts.Requests.Auth;

public sealed class RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Username { get; init; }
}