using TransactionsAPI.Models;
using TransactionsAPI.Helpers;
using TransactionsAPI.Models.Enums;

namespace TransactionsAPI.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task<PagedResult<Transaction>> GetAllAsync(int page, int pageSize);
        Task<PagedResult<Transaction>> GetByUserIdAsync(int userId, int page, int pageSize);
        Task<List<Transaction>> GetByUserWithFiltersAsync(int userId, TransactionFilterType filterType, int? month = null, int? year = null);
    }
}
