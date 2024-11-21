using Microsoft.EntityFrameworkCore;

public class BlueBankContext : DbContext
{
    public BlueBankContext(DbContextOptions<BlueBankContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> AccountBaks { get; set; }
}