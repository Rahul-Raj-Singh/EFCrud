namespace EFCruds.Model;

public class Product
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public DateTime LaunchDate  { get; set; }
    public bool IsDeleted  { get; set; }
    public List<ProductPrice> ProductPrices { get; set; }
    public string ModifiedBy  { get; set; }
    public DateTime ModifiedDateTime  { get; set; }
}
