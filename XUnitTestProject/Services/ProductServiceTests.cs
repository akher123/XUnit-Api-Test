using Api;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Moq;

namespace XUnitTestProject.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WithProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Keyboard", Price = 79.99m }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _service.GetAllAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSuccess_WhenProductExists()
    {
        var product = new Product { Id = 1, Name = "Keyboard", Price = 79.99m };
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenProductMissing()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(99, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WithCreatedProduct()
    {
        var request = new CreateProductRequest { Name = "Monitor", Price = 399.99m };
        var created = new Product { Id = 3, Name = request.Name, Price = request.Price };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var result = await _service.CreateAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Id);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenProductExists()
    {
        var existing = new Product { Id = 1, Name = "Old", Price = 10m };
        var request = new UpdateProductRequest { Name = "New", Price = 20m };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repositoryMock
            .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.UpdateAsync(1, request, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenProductMissing()
    {
        var request = new UpdateProductRequest { Name = "New", Price = 20m };
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _service.UpdateAsync(99, request, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenProductExists()
    {
        _repositoryMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenProductMissing()
    {
        _repositoryMock
            .Setup(r => r.DeleteAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.DeleteAsync(99, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }
}
