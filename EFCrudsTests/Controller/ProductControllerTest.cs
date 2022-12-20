using EFCruds.Controller;
using EFCruds.Model;
using EFCruds.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EFCrudsTests.Controller;

public class ProductControllerTest
{
    private Mock<ILogger<ProductController>> _logger;

    public ProductControllerTest()
    {
        _logger = new Mock<ILogger<ProductController>>();
    }

    [Fact]
    public void GetTest()
    {
        // Arrange
        var mockResponse = new List<Product> 
        {
            new Product {ProductName = "Router"},
            new Product {ProductName = "Watch"}
        };

        var productRepositoryMock = new Mock<ProductRepository>(null, null);
        productRepositoryMock.Setup(x => x.GetProducts(It.IsAny<string>(), It.IsAny<DateTime?>())).Returns(mockResponse);

        var sut = new ProductController(productRepositoryMock.Object, _logger.Object);

        // Act
        var result = sut.Get(null, null);

        // Assert
        Assert.IsAssignableFrom<OkObjectResult>(result);
        
        var responseBody = (List<Product>)((OkObjectResult) result).Value;
        Assert.Collection(mockResponse, 
            item1 => Assert.Equal(item1.ProductName, responseBody[0].ProductName),
            item2 => Assert.Equal(item2.ProductName, responseBody[1].ProductName)
        );
    }

    [Fact]
    public void PutTest()
    {
        // Arrange
        var productRepositoryMock = new Mock<ProductRepository>(null, null);
        productRepositoryMock.Setup(x => x.PutProducts(It.IsAny<List<Product>>()));

        var sut = new ProductController(productRepositoryMock.Object, _logger.Object);

        // Act
        var result = sut.Put(null);

        // Assert
        Assert.IsAssignableFrom<StatusCodeResult>(result);
        
        var statusCode = ((StatusCodeResult) result).StatusCode;
        Assert.Equal(201, statusCode);
    }
}