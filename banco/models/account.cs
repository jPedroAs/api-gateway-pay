public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal? Balance { get; set; } = 0;
    public decimal? SavingsBalance { get; set; } = 0;
    public int UserId { get; set; } = default;
    public User User { get; set; }
    public List<Transaction> Transactions{ get; set; }
    public bool Active { get; set; } = default;
}