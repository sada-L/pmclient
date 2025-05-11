using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.Contracts.Response.Auth;
using pmclient.Helpers;
using Refit;

namespace pmclient.RefitClients;

public interface ITwoFaWebApi
{
    [Post(ApiEndpoints.Authentication.EnableTwoFa)]
    Task<IApiResponse<string>> EnableTwoFaAsync(CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.DisableTwoFa)]
    Task<IApiResponse<AuthResponse>> DisableTwoFaAsync(CancellationToken cancellationToken);

    [Post(ApiEndpoints.Authentication.VerifyTwoFa)]
    Task<IApiResponse> VerifyTwoFaAsync([Body] ValidateRequest validateRequest, CancellationToken cancellationToken);
}