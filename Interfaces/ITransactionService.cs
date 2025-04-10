using TransactionsAPI.DTOs;
using TransactionsAPI.Helpers;
using TransactionsAPI.Models.Enums;

namespace TransactionsAPI.Interfaces
{
    public interface ITransactionService
    {
        Task<string> CreateDepositAsync(int userId, decimal amount, string descrption);
        Task<(string, string)> CreateTransferAsync(int senderId, int receiverId, decimal amount, string descrption);
        Task<UserTransactionsDTO> GetUserTransactionsAsync(int userId, int page, int pageSize);
        Task RevertTransferAsync(int transactionId);
        Task<byte[]> ExportTransactionsToCsvAsync(int userId, string filterType, int? month = null, int? year = null);
    }
}

