using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Models;
using pmclient.RefitClients;
using Splat;

namespace pmclient.Services;

public class GroupService
{
    private readonly IGroupWebApi? _groupWebApi;

    public GroupService(IGroupWebApi? groupWebApi = null)
    {
        _groupWebApi = groupWebApi ?? Locator.Current.GetService<IGroupWebApi>();
    }

    public async Task<List<Group>?> GetGroupsByUser(CancellationToken cancellationToken = default)
    {
        var response = await _groupWebApi!.GetGroupsByUser(cancellationToken);
        return response.IsSuccessStatusCode ? response.Content : null;
    }

    public async Task<int> CreateGroup(Group group, CancellationToken cancellationToken = default)
    {
        var response = await _groupWebApi!.CreateGroup(group, cancellationToken);
        return response.IsSuccessStatusCode ? response.Content : 0;
    }

    public async Task UpdateGroup(Group group, CancellationToken cancellationToken = default)
    {
        await _groupWebApi!.UpdateGroup(group, cancellationToken);
    }
}