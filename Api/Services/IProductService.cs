using Api.DTOs;
using Api.Models;

namespace Api.Services;

public interface IProductService
{
    Task<Result<IReadOnlyList<Product>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<Product>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<Product>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
