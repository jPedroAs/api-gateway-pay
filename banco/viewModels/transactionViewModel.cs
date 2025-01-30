using System.ComponentModel.DataAnnotations;
namespace banco.viewModels;

public class TransactionView
{

    [Required]
    public decimal Amount { get; set; } = default;
    public string? Cod { get; set; }
    public TransactionType Type { get; set; } = default;
}