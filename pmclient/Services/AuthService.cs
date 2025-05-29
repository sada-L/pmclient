using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using pmclient.RefitClients;
using pmclient.Settings;
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

        UserSettings.JwtToken = response.Content!.AccessToken.Replace("\"", "");
        await FileStorage.SaveFileAsync(response.Content.RefreshToken.Replace("\"", ""), "token.dat");

        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var response = await _identityWebApi!.RegisterAsync(registerRequest, cancellationToken);
        if (!response.IsSuccessStatusCode) return false;

        UserSettings.JwtToken = response.Content!.AccessToken.Replace("\"", "");
        await FileStorage.SaveFileAsync(response.Content.RefreshToken.Replace("\"", ""), "token.dat");

        return true;
    }

    public async Task<bool> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await FileStorage.LoadFileAsync("token.dat");
        if (refreshToken == null) return false;

        var request = new RefreshRequest { RefreshToken = refreshToken };
        var response = await _identityWebApi!.RefreshTokenAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return false;

        UserSettings.JwtToken = response.Content!.AccessToken.Replace("\"", "");
        await FileStorage.SaveFileAsync(response.Content.RefreshToken.Replace("\"", ""), "token.dat");

        return true;
    }

    public static async Task LogoutAsync()
    {
        UserSettings.JwtToken = null;
        UserSettings.User = null;
        await TokenStorage.DeleteTokenAsync();
    }
}