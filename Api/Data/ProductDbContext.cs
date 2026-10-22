using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Sample Keyboard", Description = "Mechanical keyboard", Price = 79.99m },
            new Product { Id = 2, Name = "Sample Mouse", Description = "Wireless mouse", Price = 29.99m });
    }
}
