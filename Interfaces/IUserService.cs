using TransactionsAPI.DTOs;
using TransactionsAPI.Helpers;
using TransactionsAPI.Models;

namespace TransactionsAPI.Interfaces
{
    public interface IUserService
    {
        Task<UserInfoDTO?> GetByIdAsync(int id);
        Task<PagedResult<UserInfoDTO>> GetAllAsync(int page, int pageSize);
        Task<string> RegisterAsync(string name, string email, string password, string postalCode);
        Task<string> DisableByIdAsync(int id);
        Task<string> LoginAsync(string email, string password);
    }
}
