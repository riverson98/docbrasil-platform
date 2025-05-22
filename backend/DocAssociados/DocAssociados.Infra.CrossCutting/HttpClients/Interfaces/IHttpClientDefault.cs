namespace DocAssociados.Service.Infra.CrossCutting.HttpClients.Interfaces;

public interface IHttpClientDefault<T> where T : class
{
    Task<T> GetDefaultAsync<T>(string endpoint);
    Task<T> PostDefaultAsync<T>(string endpoint, T body);
    Task DeleteDefaultAsync(string endpoint, Guid id);
}
