using Api.DTOs;
using Api.Models;
using Api.Repositories;

namespace Api.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => repository.GetAllAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => repository.GetByIdAsync(id, cancellationToken);

    public Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        return repository.AddAsync(product, cancellationToken);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Price = request.Price;

        return await repository.UpdateAsync(existing, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => repository.DeleteAsync(id, cancellationToken);
}
