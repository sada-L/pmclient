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

    [Post(ApiEndpoints.Groups.CreateGroup)]
    Task<IApiResponse<int>> CreateGroup([Body] Group group, CancellationToken cancellationToken);

    [Put(ApiEndpoints.Groups.UpdateGroup)]
    Task<IApiResponse> UpdateGroup(int id, [Body] Group group, CancellationToken cancellationToken);

    [Delete(ApiEndpoints.Groups.DeleteGroup)]
    Task<IApiResponse> DeleteGroup(int id, CancellationToken cancellationToken);
}