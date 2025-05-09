using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.Contracts.Response.Auth;
using pmclient.Helpers;
using Refit;

namespace pmclient.RefitClients;

public interface IIdentityWebApi
{
    [Post(ApiEndpoints.Authentication.Login)]
    Task<IApiResponse<AuthResponse>> LoginAsync([Body] LoginRequest request, CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.Register)]
    Task<IApiResponse<AuthResponse>> RegisterAsync([Body] RegisterRequest request, CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.Token)]
    Task<IApiResponse<AuthResponse>> RefreshTokenAsync([Body] RefreshRequest request, CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.EnableTwoFa)]
    Task<IApiResponse<AuthResponse>> EnableTwoFaAsync(CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.DisableTwoFa)]
    Task<IApiResponse<AuthResponse>> DisableTwoFaAsync(CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.VerifyTwoFa)]
    Task<IApiResponse<AuthResponse>> VerifyTwoFaAsync(CancellationToken cancellationToken);
}