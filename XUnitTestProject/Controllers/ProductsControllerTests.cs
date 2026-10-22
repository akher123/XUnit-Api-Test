using Api;
using Api.Controllers;
using Api.DTOs;
using Api.Models;
using Api.Products;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestProject.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new ProductsController(_productServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Keyboard", Price = 79.99m },
            new() { Id = 2, Name = "Mouse", Price = 29.99m }
        };

        _productServiceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IReadOnlyList<Product>>(okResult.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Keyboard", Price = 79.99m };
        _productServiceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returned.Id);
        Assert.Equal("Keyboard", returned.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductMissing()
    {
        // Arrange
        _productServiceMock
            .Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Product>(ProductErrors.NotFound(99)));

        // Act
        var result = await _controller.GetById(99, CancellationToken.None);

        // Assert
        var notFound = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        AssertErrorBody(notFound.Value, "Product.NotFound");
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtRoute_WhenValid()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "Monitor", Description = "4K", Price = 399.99m };
        var created = new Product { Id = 3, Name = request.Name, Description = request.Description, Price = request.Price };

        _productServiceMock
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
        var returned = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal(3, returned.Id);
        Assert.Equal("Monitor", returned.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name is required");

        var request = new CreateProductRequest { Name = "", Price = 10m };

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        _productServiceMock.Verify(
            s => s.CreateAsync(It.IsAny<CreateProductRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenProductExists()
    {
        // Arrange
        var request = new UpdateProductRequest { Name = "Updated Keyboard", Price = 89.99m };
        _productServiceMock
            .Setup(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _productServiceMock.Verify(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenProductMissing()
    {
        // Arrange
        var request = new UpdateProductRequest { Name = "Missing", Price = 10m };
        _productServiceMock
            .Setup(s => s.UpdateAsync(99, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(ProductErrors.NotFound(99)));

        // Act
        var result = await _controller.Update(99, request, CancellationToken.None);

        // Assert
        var notFound = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        AssertErrorBody(notFound.Value, "Product.NotFound");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductExists()
    {
        // Arrange
        _productServiceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _productServiceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProductMissing()
    {
        // Arrange
        _productServiceMock
            .Setup(s => s.DeleteAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(ProductErrors.NotFound(99)));

        // Act
        var result = await _controller.Delete(99, CancellationToken.None);

        // Assert
        var notFound = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        AssertErrorBody(notFound.Value, "Product.NotFound");
    }

    private static void AssertErrorBody(object? body, string expectedCode)
    {
        Assert.NotNull(body);
        var code = body.GetType().GetProperty("code")?.GetValue(body) as string;
        Assert.Equal(expectedCode, code);
    }
}
