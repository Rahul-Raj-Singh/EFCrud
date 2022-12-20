using System.Linq;
using System.Reflection.Metadata.Ecma335;
using EFCruds.DataAccess;
using EFCruds.Model;
using EFCruds.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCrudsTests.Repository;
public class ProductRepositoryTest : IDisposable
{
    private Mock<ILogger<ProductRepository>> _logger;
    private SqliteConnection _connection;
    private DbContextOptions<AppDbContext> _contextOptions;

    public ProductRepositoryTest()
    {
        _logger = new Mock<ILogger<ProductRepository>>();
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void GetFilteredQueryTest()
    {
        var products = new List<Product>
        {
            new Product {ProductName = "Iphone 14"},
            new Product {ProductName = "Nintendo Switch"},
            new Product {ProductName = "TpLink Router", LaunchDate = DateTime.Parse("2013-01-10")},
            new Product {ProductName = "Design Patterns"},
        }.AsQueryable();

        // Act
        var sut = new ProductRepository(null, null);

        var productNameFilterResult = sut.GetFilteredQuery(products,  "Switch", null);
        var launchDateFilterResult = sut.GetFilteredQuery(products,  null, DateTime.Parse("2013-01-10"));
        var noFilterResult = sut.GetFilteredQuery(products, null, null);

        // Assert
        Assert.Collection(productNameFilterResult, 
            item => Assert.Equal("Nintendo Switch", item.ProductName)
        );

        Assert.Collection(launchDateFilterResult, 
            item => Assert.Equal("TpLink Router", item.ProductName)
        );

        Assert.Equal(products.Count(), noFilterResult.Count());
    }

    [Fact]
    public void GetProductsTest()
    {
        // Arrange
        // Seed the in-memory db
        using var context = new AppDbContext(_contextOptions);

        context.Database.EnsureCreated();

        var products = new List<Product>
        {
            new Product 
            {
                ProductName = "Apple Watch", 
                ProductPrices = new List<ProductPrice>
                {
                    new ProductPrice{Variant = "v2", Price = 35000m},
                    new ProductPrice{Variant = "v1", Price = 30000m, IsDeleted = true}
                }
            },
            new Product 
            {
                ProductName = "PS2", 
                IsDeleted = true
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Act
        var sut = new ProductRepository(context, null);

        var result = sut.GetProducts(null, null);

        Assert.Collection(result,
        item => {
            Assert.Equal("Apple Watch", item.ProductName);
            Assert.Collection(item.ProductPrices, 
                item => Assert.Equal("v2", item.Variant)
            );
        });

    }

    [Fact]
    public void PutProductsTest()
    {
        // Arrange
        // Seed sqlite in-memory db
        using var context = new AppDbContext(_contextOptions);

        context.Database.EnsureCreated();

        var products = new List<Product>
        {
            new Product 
            {
                ProductName = "Apple Watch", 
                ProductPrices = new List<ProductPrice>
                {
                    new ProductPrice{Variant = "v2", Price = 35000m},
                    new ProductPrice{Variant = "v1", Price = 30000m, IsDeleted = true}
                }
            },
            new Product 
            {
                ProductName = "PS2", 
                IsDeleted = true
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Setup request body
        var sourceProducts = new List<Product>
        {
            new Product 
            {
                ProductName = "Apple Watch", 
                ProductPrices = new List<ProductPrice>
                {
                    new ProductPrice{ProductName = "Apple Watch", Variant = "v2", Price = 36000m}, // Price updated
                }
            },
            new Product 
            {
                ProductName = "PS5", // New product
                ProductPrices = new List<ProductPrice>
                {
                    new ProductPrice{ProductName = "PS5", Variant = "Digital", Price = 45000m},
                }
            }
        };

        // Act
        var sut = new ProductRepository(context, null);

        sut.PutProducts(sourceProducts);

        // Assert
        var actualResult = context.Products
        .Include(p => p.ProductPrices)
        .Where(p => !p.IsDeleted)
        .OrderBy(p => p.ProductName)
        .ToList();

        Assert.Equal(2, actualResult.Count);
        
        var watch = actualResult.FirstOrDefault(x => x.ProductName == "Apple Watch");
        var watchPrices = watch.ProductPrices.Where(pp => !pp.IsDeleted).ToList();

        Assert.NotNull(watch);
        Assert.Collection(watchPrices, x => Assert.Equal(36000m, x.Price));

        var ps = actualResult.FirstOrDefault(x => x.ProductName == "PS5");
        var psPrices = ps.ProductPrices.Where(pp => !pp.IsDeleted).ToList();

        Assert.NotNull(ps);
        Assert.Collection(psPrices, x => {
            Assert.Equal("Digital", x.Variant);
            Assert.Equal(45000m, x.Price);
        });

    }


}
