using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TransactionsAPI.Models.Enums;

namespace TransactionsAPI.Models;

public class Transaction : BaseEntity
{
    public int? SenderId { get; set; }

    [Required]
    public int ReceiverId { get; set; }

    [Required]
    [Column(TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    public DateTime? ReversedAt { get; set; }

    [ForeignKey(nameof(SenderId))]
    public User? Sender { get; set; }

    [ForeignKey(nameof(ReceiverId))]
    public User Receiver { get; set; } = null!;
}

