using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using pmclient.RefitClients;
using Splat;

namespace pmclient.Services;

public class AuthService
{
    private readonly IIdentityWebApi? _identityWebApi;

    public AuthService(IIdentityWebApi? identityWebApi = null)
    {
        _identityWebApi = identityWebApi ?? Locator.Current.GetService<IIdentityWebApi>()!;
    }

    public async Task<bool> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var response = await _identityWebApi!.LoginAsync(loginRequest, cancellationToken);
        if (!response.IsSuccessStatusCode) return false;

        StaticStorage.JwtToken = response.Content!.AccessToken.Replace("\"", "");
        await TokenStorage.SaveTokenAsync(response.Content.RefreshToken);

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var response = await _identityWebApi!.RegisterAsync(registerRequest, cancellationToken);
        if (!response.IsSuccessStatusCode) return false;

        StaticStorage.JwtToken = response.Content!.AccessToken.Replace("\"", "");
        await TokenStorage.SaveTokenAsync(response.Content.RefreshToken);

        return true;
    }

    public async Task<bool> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await TokenStorage.LoadTokenAsync();
        if (refreshToken == null) return false;

        var request = new RefreshRequest { RefreshToken = refreshToken };
        var response = await _identityWebApi!.RefreshTokenAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return false;

        StaticStorage.JwtToken = response.Content!.AccessToken.Replace("\"", "");
        await TokenStorage.SaveTokenAsync(response.Content.RefreshToken);

        return true;
    }

    public async Task LogoutAsync()
    {
        StaticStorage.JwtToken = null;
        StaticStorage.User = null;
        await TokenStorage.DeleteTokenAsync();
    }
}