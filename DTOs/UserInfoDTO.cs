using TransactionsAPI.Models;

namespace TransactionsAPI.DTOs
{
    public class UserInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string? Street { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DisabledAt { get; set; }
    }
}
