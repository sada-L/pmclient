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
    
    [Post(ApiEndpoints.Cards.AddCard)]
    Task<IApiResponse<int>> AddCard(Card card, CancellationToken cancellationToken);
}