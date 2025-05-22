using Polly;

namespace DocAssociados.Service.Infra.CrossCutting.HttpClients.Policys;

public static class HttpClientPolicys
{

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var retryDelays = new[]
        {
         TimeSpan.FromSeconds(5),
         TimeSpan.FromSeconds(20),
         TimeSpan.FromSeconds(40)
     };

        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(retryDelays);
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
    }
}
