using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Models;
using pmclient.RefitClients;
using Refit;
using Splat;

namespace pmclient.Services;

public class CardService
{
    private readonly ICardWebApi? _cardWebApi;

    public CardService(ICardWebApi? cardWebApi = null)
    {
        _cardWebApi = cardWebApi ?? Locator.Current.GetService<ICardWebApi>();
    }

    public async Task<List<Card>?> GetCardsByUser(CancellationToken cancellationToken = default)
    {
        var response = await _cardWebApi!.GetCardByUser(cancellationToken);
        return response.IsSuccessStatusCode ? response.Content : null;
    }

    public async Task<int> AddCard(Card card, CancellationToken cancellationToken = default)
    {
        var response = await _cardWebApi!.AddCard(card, cancellationToken);
        return response.IsSuccessStatusCode ? response.Content : 0;
    }
}