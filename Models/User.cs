using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionsAPI.Models;

public class User : BaseEntity
{
    #region Personal Info

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    #endregion

    #region Address

    [Required]
    [MaxLength(8)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Street { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Complement { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Neighborhood { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string State { get; set; } = string.Empty;

    #endregion

    [Column(TypeName = "numeric(18,2)")]
    public decimal Balance { get; set; } = 0;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DisabledAt { get; set; }

    public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();

    public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
}
