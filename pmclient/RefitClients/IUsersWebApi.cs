using System.Threading;
using System.Threading.Tasks;
using pmclient.Helpers;
using pmclient.Models;
using Refit;

namespace pmclient.RefitClients;

[Headers("Bearer")]
public interface IUsersWebApi
{
    [Get(ApiEndpoints.Users.GetCurrentUser)]
    Task<IApiResponse<User>> GetCurrentUser(CancellationToken cancellationToken);

    [Put(ApiEndpoints.Users.UpdateCurrentUser)]
    Task<IApiResponse> UpdateCurrentUser([Body] User user, CancellationToken cancellationToken);

    [Delete(ApiEndpoints.Users.DeleteCurrentUser)]
    Task<IApiResponse> DeleteCurrentUser(CancellationToken cancellationToken);
}