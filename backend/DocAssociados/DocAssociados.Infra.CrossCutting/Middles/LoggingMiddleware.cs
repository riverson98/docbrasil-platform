using DocAssociados.Service.Infra.CrossCutting.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DocAssociados.Service.Infra.CrossCutting.Middles;
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerService>();
        var request = context.Request;
        logger.Info($"[Request] {request.Method} {request.Path} - {request.ContentType}");

        if (request.ContentLength > 0 && request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            logger.Info($"[Request Body] {body}");
            request.Body.Seek(0, SeekOrigin.Begin);
        }
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Erro inesperado no middleware");
            throw;
        }

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        logger.Info($"[Response] {context.Response.StatusCode} - {responseBodyText}");

        await responseBody.CopyToAsync(originalBodyStream);
    }
}
