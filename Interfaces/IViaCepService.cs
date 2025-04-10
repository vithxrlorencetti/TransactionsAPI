using TransactionsAPI.DTOs;

namespace TransactionsAPI.Interfaces;

public interface IViaCepService
{
    Task<ViaCepResponseDTO?> GetAddressByCepAsync(string cep);
}
