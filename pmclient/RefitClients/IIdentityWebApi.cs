using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using Refit;

namespace pmclient.RefitClients;

public interface IIdentityWebApi
{
  [Post(ApiEndpoints.Authentication.Login)]
  Task<IApiResponse<string>> LoginAsync(LoginRequest request, CancellationToken cancellationToken );
  
  [Post(ApiEndpoints.Authentication.Register)]
  Task<IApiResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken );
}