using System.Diagnostics.CodeAnalysis;
using EFCruds.Model;
using Microsoft.EntityFrameworkCore;

namespace EFCruds.DataAccess;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _config;
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductPrice> ProductPrices { get; set; }

    // To be used in unit tests
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {}

    // To be used in app
    public AppDbContext(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_config.GetConnectionString("MyConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
        .ToTable("Product")
        .HasKey(x => x.ProductName);

        modelBuilder.Entity<Product>()
        .HasMany(x => x.ProductPrices)
        .WithOne(y => y.Product)
        .HasForeignKey(y => y.ProductName);

        modelBuilder.Entity<ProductPrice>()
        .ToTable("ProductPrice")
        .HasKey(x => new {x.ProductName, x.Variant});

    }
}
