using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.RefitClients;

namespace pmclient.Services;

public class TwoFaService
{
    private readonly ITwoFaWebApi? _twoFaWebApi;

    public TwoFaService(ITwoFaWebApi? twoFaWebApi)
    {
        _twoFaWebApi = twoFaWebApi;
    }

    public async Task EnableTwoFaAsync(CancellationToken cancellationToken)
    {
        var response = await _twoFaWebApi!.EnableTwoFaAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) return;

        UserSettings.User!.Secret = response.Content!.Replace("\"", "");
    }

    public async Task DisableTwoFaAsync(CancellationToken cancellationToken)
    {
        var response = await _twoFaWebApi!.DisableTwoFaAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) return;

        UserSettings.User!.Secret = null;
    }

    public async Task<bool> VerifyTwoFaAsync(ValidateRequest request, CancellationToken cancellationToken)
    {
        var response = await _twoFaWebApi!.VerifyTwoFaAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}