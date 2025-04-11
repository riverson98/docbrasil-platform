using DocAssociados.ApiGateway.Config;
using System.Diagnostics;

namespace DocAssociados.ApiGateway.Handlers;

public class AssociadoApiKeyHandler : DelegatingHandler
{
    private readonly ApiKeyContainer _keys;

    public AssociadoApiKeyHandler(ApiKeyContainer keys)
    {
        _keys = keys;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-Api-Key", _keys.AssociadoKey);
        return await base.SendAsync(request, cancellationToken);
    }
}
