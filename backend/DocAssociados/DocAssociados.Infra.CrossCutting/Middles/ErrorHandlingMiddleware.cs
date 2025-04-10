using DocAssociados.Service.Infra.CrossCutting.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DocAssociados.Service.Infra.CrossCutting.Middles;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var logger = httpContext.RequestServices.GetRequiredService<ILoggerService>();
            logger.Error(ex, "Ocorreu um erro inesperado.");
            throw;
        }
    }
}
