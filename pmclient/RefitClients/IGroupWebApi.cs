using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Helpers;
using pmclient.Models;
using Refit;

namespace pmclient.RefitClients;

[Headers("Bearer")]
public interface IGroupWebApi
{
    [Get(ApiEndpoints.Groups.GetGroupsByUser)]
    Task<IApiResponse<List<Group>>> GetGroupsByUser(CancellationToken cancellationToken);
    
    [Post(ApiEndpoints.Groups.AddGroup)]
    Task<IApiResponse<int>> AddGroup(Group group, CancellationToken cancellationToken = default);
}