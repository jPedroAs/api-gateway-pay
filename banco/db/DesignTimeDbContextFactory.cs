using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BlueBankContext>
{
    public BlueBankContext CreateDbContext(string[] args)
    {
        // var config = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //     .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BlueBankContext>();
        optionsBuilder.UseNpgsql("postgresql://banco_gh05_user:M99HADj2qIBjpePTPV9O3mNhYSFvYGSJ@dpg-cuj7ektumphs738bgfs0-a/banco_gh05");

        return new BlueBankContext(optionsBuilder.Options);
    }
}
