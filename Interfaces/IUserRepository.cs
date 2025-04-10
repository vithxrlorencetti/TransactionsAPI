using TransactionsAPI.Helpers;
using TransactionsAPI.Models;

namespace TransactionsAPI.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<PagedResult<User>> GetAllAsync(int page, int pageSize);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
}
