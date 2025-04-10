using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace DocAssociados.Service.Infra.CrossCutting.Logs;

public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }
    public void Error(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
                     [CallerLineNumber] int lineNumber = 0)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // ou DateTime.UtcNow
        _logger.LogInformation("[{Timestamp}] [{File}] {Method} (linha {Line}): {Message}",
            timestamp, Path.GetFileName(filePath), memberName, lineNumber, message);
    }

    public void Info(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
                    [CallerLineNumber] int lineNumber = 0)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _logger.LogInformation("[{Timestamp}] [{File}] {Method} (linha {Line}): {Message}",
            timestamp, Path.GetFileName(filePath), memberName, lineNumber, message);
    }
}
