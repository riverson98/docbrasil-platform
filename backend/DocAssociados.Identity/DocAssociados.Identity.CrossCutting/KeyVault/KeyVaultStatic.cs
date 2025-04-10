using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DocAssociados.Identity.Infra.CrossCutting.Config;

namespace DocAssociados.Identity.Infra.CrossCutting.KeyVault;

public class KeyVaultStatic
{
    private static SecretClient _client;
    private static string _kvUri;

    public static void Init(AzureVaultConfig keyVaultConfig)
    {
        _kvUri = $"https://{keyVaultConfig.KeyVaultUrl}.vault.azure.net/";
        _client = new SecretClient(new Uri(_kvUri), new DefaultAzureCredential());
    }


    public static async Task<string> GetSecretAsync(string name)
    {
        if (_client == null)
            throw new InvalidOperationException("KeyVaultStatic nao inicializada. chame o Init() primeiro.");
        var secret = await _client.GetSecretAsync(name);
        return secret.Value.Value;
    }
}
