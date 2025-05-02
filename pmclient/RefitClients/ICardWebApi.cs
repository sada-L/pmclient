using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Helpers;
using pmclient.Models;
using Refit;

namespace pmclient.RefitClients;

[Headers("Bearer")]
public interface ICardWebApi
{
    [Get(ApiEndpoints.Cards.GetCardsByUser)]
    Task<IApiResponse<List<Card>>> GetCardByUser(CancellationToken cancellationToken);

    [Post(ApiEndpoints.Cards.CreateCard)]
    Task<IApiResponse<int>> CreateCard([Body] Card card, CancellationToken cancellationToken);

    [Put(ApiEndpoints.Cards.UpdateCard)]
    Task<IApiResponse> UpdateCard(int id, [Body] Card card, CancellationToken cancellationToken);

    [Delete(ApiEndpoints.Cards.DeleteCard)]
    Task<IApiResponse> DeleteCard(int id, CancellationToken cancellationToken);
}