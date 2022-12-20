using Microsoft.EntityFrameworkCore;
using EFCruds.DataAccess;
using EFCruds.Model;

namespace EFCruds.Repository;

public class ProductRepository
{
    private readonly ILogger<ProductRepository> logger;
    private readonly AppDbContext dbContext;
    public ProductRepository(AppDbContext dbContext, ILogger<ProductRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;

    }
    public virtual List<Product> GetProducts(string productName, DateTime? launchDate)
    {
        var query = dbContext.Products
        .Where(p => !p.IsDeleted)
        .Include(p => p.ProductPrices)
        // .Include(p => p.ProductPrices.Where(pp => !pp.IsDeleted))
        .AsQueryable();

        query = GetFilteredQuery(query, productName, launchDate);

        var products = query.ToList();

        // Client side filtering, as server side include-filtering is failing with SQLite
        foreach(var product in products)
        {
            product.ProductPrices = product.ProductPrices.Where(pp => !pp.IsDeleted).ToList();
        }

        return products;
    }
    public virtual void PutProducts(List<Product> sourceProducts)
    {
        var sourceProductPrices = sourceProducts.SelectMany(p => p.ProductPrices).ToList();

        // Upsert Products
        foreach(var source in sourceProducts)
        {
            // Avoid automatic cascade save
            source.ProductPrices = null;

            var target = dbContext.Products.FirstOrDefault(p => p.ProductName == source.ProductName);

            if (target is null)
            {
                dbContext.Products.Add(source);
            }
            else
            {
                target.Description = source.Description;
                target.LaunchDate = source.LaunchDate;
                target.IsDeleted = source.IsDeleted;
                target.ModifiedBy = source.ModifiedBy;
                target.ModifiedDateTime = source.ModifiedDateTime;
            }
        }

        // Upsert Product Prices
        foreach(var source in sourceProductPrices)
        {
            // Avoid automatic cascade save
            source.Product = null;

            var target = dbContext.ProductPrices
            .FirstOrDefault(pp => pp.ProductName == source.ProductName && pp.Variant == source.Variant);

            if (target is null)
            {
                dbContext.ProductPrices.Add(source);
            }
            else
            {
                target.Price = source.Price;
                target.IsDeleted = source.IsDeleted;
                target.ModifiedBy = source.ModifiedBy;
                target.ModifiedDateTime = source.ModifiedDateTime;
            }
        }

        // Transactional save
        dbContext.SaveChanges();
    }

    public IQueryable<Product> GetFilteredQuery(IQueryable<Product> query, string productName, DateTime? launchDate)
    {
        if (productName != null)
            query = query.Where(p => p.ProductName.ToLower().Contains(productName.ToLower()));
        
        if (launchDate != null)
            query = query.Where(p => p.LaunchDate == launchDate);

        return query;
    }
}
