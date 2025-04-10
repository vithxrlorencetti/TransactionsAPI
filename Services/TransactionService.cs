using System.Text;
using AutoMapper;
using TransactionsAPI.DTOs;
using TransactionsAPI.Helpers;
using TransactionsAPI.Interfaces;
using TransactionsAPI.Models;
using TransactionsAPI.Models.Enums;

namespace TransactionsAPI.Services;

public class TransactionService : ITransactionService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionService(IUserRepository userRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<string> CreateDepositAsync(int userId, decimal amount, string description)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new NotFoundException("User not found");

            user.Balance += amount;

            var transaction = new Transaction
            {
                ReceiverId = userId,
                Amount = amount,
                Type = TransactionType.Deposit,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return user.Name;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }

    public async Task<(string, string)> CreateTransferAsync(int senderId, int receiverId, decimal amount, string description)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            if (senderId == receiverId)
                throw new BadRequestException("Cannot transfer to the same user");

            var sender = await _userRepository.GetByIdAsync(senderId)
                         ?? throw new NotFoundException("Sender not found");

            var receiver = await _userRepository.GetByIdAsync(receiverId)
                           ?? throw new NotFoundException("Receiver not found");

            if (sender.Balance < amount)
                throw new BadRequestException("Insufficient balance");

            sender.Balance -= amount;
            receiver.Balance += amount;

            var transaction = new Transaction
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Amount = amount,
                Type = TransactionType.Transfer,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return (sender.Name, receiver.Name);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }

    public async Task RevertTransferAsync(int transactionId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var transaction = await _transactionRepository.GetByIdAsync(transactionId)
                              ?? throw new NotFoundException("Transaction not found");

            if (transaction.Type != TransactionType.Transfer)
                throw new BadRequestException("Only transfers can be reverted");

            if (transaction.ReversedAt != null)
                throw new ConflictException("Transfer already reverted");

            var sender = await _userRepository.GetByIdAsync(transaction.SenderId!.Value);
            var receiver = await _userRepository.GetByIdAsync(transaction.ReceiverId);

            if (receiver!.Balance < transaction.Amount)
                throw new BadRequestException("Receiver does not have enough balance to revert");

            receiver.Balance -= transaction.Amount;
            sender!.Balance += transaction.Amount;
            transaction.ReversedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }

    public async Task<UserTransactionsDTO> GetUserTransactionsAsync(int userId, int page, int pageSize)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new NotFoundException("User not found");

        var pagedTransactions = await _transactionRepository.GetByUserIdAsync(userId, page, pageSize);

        var userDto = _mapper.Map<UserInfoDTO>(user);
        var transactionDtos = _mapper.Map<List<TransactionReadDTO>>(pagedTransactions.Items);

        return new UserTransactionsDTO
        {
            User = userDto,
            Transactions = new PagedResult<TransactionReadDTO>
            {
                Items = transactionDtos,
                TotalCount = pagedTransactions.TotalCount,
                Page = pagedTransactions.Page,
                PageSize = pagedTransactions.PageSize
            }
        };
    }

    public async Task<byte[]> ExportTransactionsToCsvAsync(int userId, string filterType, int? month = null, int? year = null)
    {
        var filterEnum = filterType.ToLower() switch
        {
            "last30days" => TransactionFilterType.Last30Days,
            "bymonth" when month.HasValue && year.HasValue => TransactionFilterType.ByMonth,
            "reversed" => TransactionFilterType.Reversed,
            "all" => TransactionFilterType.All,
            _ => throw new BadRequestException("Invalid filter type")
        };

        var transactions = await _transactionRepository.GetByUserWithFiltersAsync(userId, filterEnum, month, year);

        var sb = new StringBuilder();
        sb.AppendLine("Id,Date,Type,Amount,Sender,Receiver");

        foreach (var t in transactions)
        {
            sb.AppendLine($"{t.Id},{t.CreatedAt:yyyy-MM-dd HH:mm},{t.Type},{t.Amount},{t.Sender?.Name},{t.Receiver.Name}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
