using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using Refit;

namespace pmclient.RefitClients;

public interface IIdentityWebApi
{
  [Post(ApiEndpoints.Authentication.Login)]
  Task<IApiResponse<string>> LoginAsync([Body]LoginRequest request, CancellationToken cancellationToken );
  
  [Post(ApiEndpoints.Authentication.Register)]
  Task<IApiResponse<string>> RegisterAsync([Body]RegisterRequest request, CancellationToken cancellationToken );
}