using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.Core;

public class ShoppingDbContext : DbContext
{
    public ShoppingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin",
                SaltPassword = "80VbTKrTBV5+ikH4zlgOuw==",
                HashPassword = "aYtoUOeT560j00mu1Q6+oIVfDSA=",
                IsAdmin = true
            }
        );
    }
}

public class ShoppingDbContextFactory : IDesignTimeDbContextFactory<ShoppingDbContext>
{
    public ShoppingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShoppingDbContext>();
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-FM9EFRD;Initial Catalog=ProductDbContext;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

        return new ShoppingDbContext(optionsBuilder.Options);
    }
}
