using Microsoft.EntityFrameworkCore;
using TransactionsAPI.Data;
using TransactionsAPI.Helpers;
using TransactionsAPI.Interfaces;
using TransactionsAPI.Models;
using TransactionsAPI.Models.Enums;

namespace TransactionsAPI.Repositories
{
    public class TransactionRepository(AppDbContext context) : ITransactionRepository
    {
        public async Task AddAsync(Transaction transaction)
        {
            await context.Transactions.AddAsync(transaction);
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await context.Transactions
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<PagedResult<Transaction>> GetAllAsync(int page, int pageSize)
        {
            var query = context.Transactions
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Transaction>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Transaction>> GetByUserIdAsync(int userId, int page, int pageSize)
        {
            var query = context.Transactions
                .Where(t => t.SenderId == userId || t.ReceiverId == userId)
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Transaction>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<Transaction>> GetByUserWithFiltersAsync(int userId, TransactionFilterType filterType, int? month = null, int? year = null)
        {
            var query = context.Transactions
                .Where(t => t.SenderId == userId || t.ReceiverId == userId)
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .AsQueryable();

            switch (filterType)
            {
                case TransactionFilterType.Last30Days:
                    var period = DateTime.UtcNow.AddDays(-30);
                    query = query.Where(t => t.CreatedAt >= period);
                    break;

                case TransactionFilterType.ByMonth:
                    if (!month.HasValue || !year.HasValue)
                        throw new ArgumentException("Month and year must be provided when using the 'ByMonth' filter.");

                    query = query.Where(t =>
                        t.CreatedAt.Month == month.Value &&
                        t.CreatedAt.Year == year.Value);
                    break;

                case TransactionFilterType.Reversed:
                    query = query.Where(t => t.ReversedAt != null);
                    break;

                case TransactionFilterType.All:
                default:
                    break;
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

    }
}
