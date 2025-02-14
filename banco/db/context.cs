using Microsoft.EntityFrameworkCore;

public class BlueBankContext : DbContext
{
    public BlueBankContext(DbContextOptions<BlueBankContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> AccountBaks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id); 

            entity.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); 

            entity.Property(t => t.Date)
                .IsRequired(); 

            entity.Property(t => t.Type)
                .IsRequired(); 

            entity.HasOne(t => t.Account) 
                .WithMany(a => a.Transactions) 
                .HasForeignKey(t => t.AccountId) 
                .OnDelete(DeleteBehavior.Cascade); 

             entity.HasOne(t => t.Account) 
                .WithMany(a => a.Transactions) 
                .HasForeignKey(t => t.Account_transferred) 
                .OnDelete(DeleteBehavior.Cascade); 
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id); 

            entity.Property(u => u.Username)
                .IsRequired() 
                .HasMaxLength(100); 

            entity.Property(u => u.Password)
                .IsRequired() 
                .HasMaxLength(200); 
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.AccountNumber)
                .IsRequired() 
                .HasMaxLength(50); 

            entity.Property(a => a.Balance)
                .HasColumnType("decimal(18,2)"); 

            entity.Property(a => a.SavingsBalance)
                .HasColumnType("decimal(18,2)"); 

            // Relacionamento com User
            entity.HasOne(a => a.User)
                .WithMany() 
                .HasForeignKey(a => a.UserId) 
                .OnDelete(DeleteBehavior.Restrict); 

            // Relacionamento com Transaction
            entity.HasMany(a => a.Transactions) 
                .WithOne(t => t.Account) 
                .HasForeignKey(t => t.AccountId) 
                .OnDelete(DeleteBehavior.Cascade); 
            
            entity.HasMany(a => a.Transactions) 
                .WithOne(t => t.Account) 
                .HasForeignKey(t => t.Account_transferred) 
                .OnDelete(DeleteBehavior.Cascade); 
        });
    }
}