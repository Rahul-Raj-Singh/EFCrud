using Newtonsoft.Json;

namespace EFCruds.Model;

public class ProductPrice
{
    public string ProductName { get; set; }
    public string Variant { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted  { get; set; }
    public Product Product { get; set; }
    public string ModifiedBy  { get; set; }
    public DateTime ModifiedDateTime  { get; set; }
}
