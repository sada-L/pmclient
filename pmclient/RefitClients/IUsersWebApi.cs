using System.Threading;
using System.Threading.Tasks;
using pmclient.Helpers;
using pmclient.Models;
using Refit;

namespace pmclient.RefitClients;

public interface IUsersWebApi
{
    [Get(ApiEndpoints.Users.GetUserByEmail)]
    Task<IApiResponse<User>> GetUserByEmail(string email, CancellationToken cancellationToken);
}