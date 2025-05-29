using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using pmclient.Settings;

namespace pmclient.HttpHandlers;

public sealed class ApiKeyAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ApiKeySettings _apiKeySettings;

    public ApiKeyAuthorizationMessageHandler(IOptions<ApiKeySettings> apiKeySettings)
    {
        _apiKeySettings = apiKeySettings.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add(_apiKeySettings.HeaderName, _apiKeySettings.ApiKey);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}