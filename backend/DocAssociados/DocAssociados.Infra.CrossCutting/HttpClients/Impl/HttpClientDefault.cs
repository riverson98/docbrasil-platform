using DocAssociados.Service.Infra.CrossCutting.HttpClients.Interfaces;
using System.Text;
using System.Text.Json;

namespace DocAssociados.Service.Infra.CrossCutting.HttpClients.Impl;

public class HttpClientDefault<T> : IHttpClientDefault<T> where T : class
{
    protected readonly HttpClient _httpClientDefault;

    public HttpClientDefault(IHttpClientFactory httpClientFactory)
    {
        _httpClientDefault = httpClientFactory.CreateClient("DefaultHttpClient");
    }

    public async Task DeleteDefaultAsync(string endpoint, Guid id)
    {
        var formatedEnnpoint = $"{endpoint}/{id}";
        var response = await _httpClientDefault.DeleteAsync(formatedEnnpoint);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T1> GetDefaultAsync<T1>(string endpoint)
    {
        var response = await _httpClientDefault.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T1>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<T1> PostDefaultAsync<T1>(string endpoint, T1 body)
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClientDefault.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T1>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}
