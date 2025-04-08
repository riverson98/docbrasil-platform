using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DocAssociados.Service.Infra.CrossCutting.Middles;
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        _logger.LogInformation($"[Request] {request.Method} {request.Path} - {request.ContentType}");

        if (request.ContentLength > 0 && request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            _logger.LogInformation($"[Request Body] {body}");
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
            _logger.LogError(ex, "Erro inesperado no middleware");
            throw;
        }

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        _logger.LogInformation($"[Response] {context.Response.StatusCode} - {responseBodyText}");

        await responseBody.CopyToAsync(originalBodyStream);
    }
}
