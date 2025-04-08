using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DocAssociados.Service.Infra.CrossCutting.Config;

namespace DocAssociados.Service.Infra.CrossCutting.AzureIdentity;

public class KeyVaultService : IKeyVaultService
{
    private readonly SecretClient _client;

    public KeyVaultService(AzureVaultConfig keyVaultConfig)
    {
        var kvUri = $"https://{keyVaultConfig.KeyVaultUrl}.vault.azure.net/";
        _client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
    }

    public async Task<string> GetSecretAsync(string name)
    {
        var secret = await _client.GetSecretAsync(name);
        return secret.Value.Value;
    }
}
