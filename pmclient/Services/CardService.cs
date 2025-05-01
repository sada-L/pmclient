using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Models;
using pmclient.RefitClients;
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

    public async Task<int> CreateCard(Card card, CancellationToken cancellationToken = default)
    {
        var response = await _cardWebApi!.CreateCard(card, cancellationToken);
        return response.IsSuccessStatusCode ? response.Content : 0;
    }

    public async Task UpdateCard(Card card, CancellationToken cancellationToken = default)
    {
        await _cardWebApi!.UpdateCard(card, cancellationToken);
    }
}