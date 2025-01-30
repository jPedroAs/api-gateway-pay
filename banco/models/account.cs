public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = "";
    public decimal? Balance { get; set; } = null;
    public decimal? SavingsBalance { get; set; } = null;
    public int UserId { get; set; }
    public User User { get; set; } = new User();
    public int TransactionId { get; set;}
    public List<Transaction> Transactions{ get; set; } = new List<Transaction>();
}