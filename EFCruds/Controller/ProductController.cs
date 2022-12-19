using System.Net;
using EFCruds.Model;
using EFCruds.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EFCruds.Controller;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductRepository productRepository;
    private readonly ILogger<ProductController> logger;

    public ProductController(ProductRepository productRepository, ILogger<ProductController> logger)
    {
        this.productRepository = productRepository;
        this.logger = logger;
    }

    [HttpGet("GetProducts")]
    public IActionResult Get([FromQuery] string productName, [FromQuery] DateTime? launchDate)
    {

        var products = productRepository.GetProducts(productName, launchDate);
        
        return Ok(products);
    }    
    
    [HttpPut("PutProducts")]
    public IActionResult Put([FromBody] List<Product> sourceProducts)
    {
        productRepository.PutProducts(sourceProducts);

        return StatusCode((int) HttpStatusCode.Created);
    }
}
