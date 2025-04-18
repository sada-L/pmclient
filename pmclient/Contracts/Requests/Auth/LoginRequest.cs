namespace pmclient.Contracts.Requests.Auth;

public sealed class LoginRequest
{
   public string Username { get; set; } = null!;
   
   public string Password { get; set; } = null!;
}