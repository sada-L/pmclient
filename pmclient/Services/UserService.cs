using System.Threading;
using System.Threading.Tasks;
using pmclient.Models;
using pmclient.RefitClients;

namespace pmclient.Services;

public class UserService
{
    private readonly IUsersWebApi _usersWebApi;

    public UserService(IUsersWebApi usersWebApi)
    {
        _usersWebApi = usersWebApi;
    }

    public async Task<bool> GetUserAsync(CancellationToken cancellationToken)
    {
       var response = await _usersWebApi.GetCurrentUser(cancellationToken);
       if (!response.IsSuccessStatusCode) return false;
       
       StaticStorage.User = response.Content!;
       return true;
    }
}
