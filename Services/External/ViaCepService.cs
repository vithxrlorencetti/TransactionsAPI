using TransactionsAPI.DTOs;
using TransactionsAPI.Interfaces;

namespace TransactionsAPI.Services.External;

public class ViaCepService(IHttpClientFactory httpClientFactory) : IViaCepService
{
    public async Task<ViaCepResponseDTO?> GetAddressByCepAsync(string cep)
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");

        if (!response.IsSuccessStatusCode)
            return null;

        var address = await response.Content.ReadFromJsonAsync<ViaCepResponseDTO>();
        return address?.Error == true ? null : address;
    }
}
