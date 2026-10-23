using Api;
using Api.Controllers;
using Api.Discounts;
using Api.DTOs;
using Api.Models;
using Api.Products;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace XUnitTestProject.Controllers
{
    public class DiscountsControllerTest
    {
        private readonly Mock<IDiscountService> _discountServiceMock;
        private readonly DiscountsController _controller;
        public DiscountsControllerTest()
        {
            _discountServiceMock = new Mock<IDiscountService>();
            _controller = new DiscountsController(_discountServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithDiscounts()
        {
            // Arrange

            var discount = new List<Discount>()
            {
                new Discount(){Id=1, Name="Keyboard",Percentage=2,ProductId=1},
                new Discount(){Id=1, Name="Mouse",Percentage=3,ProductId=2}
            };

            _discountServiceMock
                .Setup(s=>s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(discount);

            // Act

            var result = await _controller.GetAll(CancellationToken.None);

            // Assert

            var okResult=Assert.IsType<OkObjectResult>(result.Result);

            var returend = Assert.IsAssignableFrom<IReadOnlyList<Discount>>(okResult.Value);

            Assert.Equal(2, returend.Count);

        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenDiscountExists()
        {
            // Arrange 

            var discount = new Discount()
            {
                Id = 1,
                Name = "Keyboard",
                Percentage = 2,
                ProductId = 1
            };

            _discountServiceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(discount);

            //Act

            var result=await _controller.GetById(1,CancellationToken.None);

            //Assert

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returend = Assert.IsType<Discount>(okResult.Value);
            Assert.Equal(1, returend.Id);
            Assert.Equal(1, returend.ProductId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDiscountMissing()
        {
            // Arrange
            _discountServiceMock
                .Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<Discount>(DiscountErrors.NotFound(99)));

            // Act
            var result = await _controller.GetById(99, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
            AssertErrorBody(notFound.Value, "Discount.NotFound");
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtRoute_WhenValid()
        {
            // Arrange
            var request = new CreateDiscountRequest { ProductId = 1, Name = "Launch discount", Percentage = 10m };
            var created = new Discount { Id = 3, ProductId = request.ProductId, Name = request.Name, Percentage = request.Percentage };

            _discountServiceMock
                .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(DiscountsController.GetById), createdResult.ActionName);
            var returned = Assert.IsType<Discount>(createdResult.Value);
            Assert.Equal(3, returned.Id);
            Assert.Equal("Launch discount", returned.Name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");

            var request = new CreateDiscountRequest { Name = "", ProductId = 1, Percentage = 10m };

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            _discountServiceMock.Verify(
                s => s.CreateAsync(It.IsAny<CreateDiscountRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenProductMissing()
        {
            // Arrange
            var request = new CreateDiscountRequest { ProductId = 99, Name = "Missing product", Percentage = 10m };
            _discountServiceMock
                .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<Discount>(ProductErrors.NotFound(99)));

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
            AssertErrorBody(notFound.Value, "Product.NotFound");
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenDiscountExists()
        {
            // Arrange
            var request = new UpdateDiscountRequest { ProductId = 1, Name = "Updated discount", Percentage = 15m };
            _discountServiceMock
                .Setup(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.Update(1, request, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _discountServiceMock.Verify(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenDiscountMissing()
        {
            // Arrange
            var request = new UpdateDiscountRequest { ProductId = 1, Name = "Missing", Percentage = 10m };
            _discountServiceMock
                .Setup(s => s.UpdateAsync(99, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure(DiscountErrors.NotFound(99)));

            // Act
            var result = await _controller.Update(99, request, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
            AssertErrorBody(notFound.Value, "Discount.NotFound");
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");

            var request = new UpdateDiscountRequest { Name = "", ProductId = 1, Percentage = 10m };

            // Act
            var result = await _controller.Update(1, request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _discountServiceMock.Verify(
                s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateDiscountRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDiscountExists()
        {
            // Arrange
            _discountServiceMock
                .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.Delete(1, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _discountServiceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenDiscountMissing()
        {
            // Arrange
            _discountServiceMock
                .Setup(s => s.DeleteAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure(DiscountErrors.NotFound(99)));

            // Act
            var result = await _controller.Delete(99, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
            AssertErrorBody(notFound.Value, "Discount.NotFound");
        }

        private static void AssertErrorBody(object? body, string expectedCode)
        {
            Assert.NotNull(body);
            var code = body.GetType().GetProperty("code")?.GetValue(body) as string;
            Assert.Equal(expectedCode, code);
        }

    }
}
