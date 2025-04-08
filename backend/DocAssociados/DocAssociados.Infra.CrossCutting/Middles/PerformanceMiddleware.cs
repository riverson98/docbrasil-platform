using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DocAssociados.Service.Infra.CrossCutting.Middles;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(httpContext);
        stopwatch.Stop();
        _logger.LogInformation("Tempo de resposta para {Path}: {ElapsedMilliseconds}ms", httpContext.Request.Path, stopwatch.ElapsedMilliseconds);
    }
}
