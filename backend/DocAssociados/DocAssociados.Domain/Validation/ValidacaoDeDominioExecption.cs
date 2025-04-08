namespace DocAssociados.Domain.Validation;

public class ValidacaoDeDominioExecption : Exception
{
    public ValidacaoDeDominioExecption(string error) : base(error)
        { }
       
    public static void When(bool hasError, string errorMessage)
    {
        
        if (hasError)
            throw new ValidacaoDeDominioExecption(errorMessage);
    }
}
