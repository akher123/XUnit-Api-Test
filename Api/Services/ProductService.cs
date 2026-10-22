using Api.DTOs;
using Api.Models;
using Api.Products;
using Api.Repositories;

namespace Api.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<Result<IReadOnlyList<Product>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return Result.Success(products);
    }

    public async Task<Result<Product>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<Product>(ProductErrors.NotFound(id));
        }

        return product;
    }

    public async Task<Result<Product>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        return await repository.AddAsync(product, cancellationToken);
    }

    public async Task<Result> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return Result.Failure(ProductErrors.NotFound(id));
        }

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Price = request.Price;

        var updated = await repository.UpdateAsync(existing, cancellationToken);
        if (!updated)
        {
            return Result.Failure(ProductErrors.NotFound(id));
        }

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Result.Failure(ProductErrors.NotFound(id));
        }

        return Result.Success();
    }
}
