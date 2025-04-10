using DocAssociados.Service.Infra.CrossCutting.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace DocAssociados.Service.Infra.CrossCutting.Middles;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;

    public PerformanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var logger = httpContext.RequestServices.GetRequiredService<ILoggerService>();
        var stopwatch = Stopwatch.StartNew();
        await _next(httpContext);
        stopwatch.Stop();
        logger.Info($"Tempo de resposta para {httpContext.Request.Path}: {stopwatch.ElapsedMilliseconds}ms");
    }
}
