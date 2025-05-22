namespace DocAssociados.Domain.Validation;

public class ValidacaoDeDominioException : Exception
{
    public ValidacaoDeDominioException(string error) : base(error)
        { }
       
    public static void When(bool hasError, string errorMessage)
    {
        
        if (hasError)
            throw new ValidacaoDeDominioException(errorMessage);
    }
}
