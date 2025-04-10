using Microsoft.EntityFrameworkCore;
using TransactionsAPI.Data;
using TransactionsAPI.Helpers;
using TransactionsAPI.Interfaces;
using TransactionsAPI.Models;

namespace TransactionsAPI.Repositories.Interfaces;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id) =>
        await context.Users
            .Include(t => t.SentTransactions)
            .Include(t => t.ReceivedTransactions)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<PagedResult<User>> GetAllAsync(int page, int pageSize)
    {
        var query = context.Users
            .Include(t => t.SentTransactions)
            .Include(t => t.ReceivedTransactions)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}

