using TransactionsAPI.Helpers;

namespace TransactionsAPI.DTOs
{
    public class UserTransactionsDTO
    {
        public UserInfoDTO User { get; set; } = null!;
        public PagedResult<TransactionReadDTO> Transactions { get; set; } = null!;
    }
}
