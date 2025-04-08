namespace DocAssociados.Service.Infra.CrossCutting.AzureIdentity;
public interface IKeyVaultService
{
    Task<string> GetSecretAsync(string name);
}
