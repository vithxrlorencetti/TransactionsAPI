namespace TransactionsAPI.DTOs
{
    public class DepositDTO
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
