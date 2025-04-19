namespace pmclient.Contracts.Requests.Auth;

public sealed class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Username { get; set; }
}