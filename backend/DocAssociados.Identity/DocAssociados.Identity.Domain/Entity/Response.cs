using DocAssociados.Identity.Domain.Validation;

namespace DocAssociados.Identity.Domain.Entity;

public class Response
{
    public string? Id { get; private set; }
    public string? Email { get; private set; }
    public List<string>? Errors { get; private set; }
    public bool Success { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Response()
    {

    }

    public Response(bool success, string? id, string? email)
    {
        if (success)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(id), "The user id can be empty or null");
            DomainExceptionValidation.When(string.IsNullOrEmpty(email), "The user email can be empty or null");

            CreatedAt = DateTime.Now;
        }

        Id = id;
        Success = success;
        Email = email;
    }

    public Response(List<string> errors, bool success)
    {
        if (errors is null)
            Errors = new List<string>();

        Success = success;
        Errors = errors;
    }

    public bool GetSuccess()
    {
        return Success;
    }

    public List<string>? GetErrors()
    {
        return Errors;
    }
}
