using System.Runtime.CompilerServices;

namespace DocAssociados.Service.Infra.CrossCutting.Logs;

public interface ILoggerService
{
    void Info(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
                    [CallerLineNumber] int lineNumber = 0);
    void Error(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
                     [CallerLineNumber] int lineNumber = 0);
}
