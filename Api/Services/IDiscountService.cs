using Api.DTOs;
using Api.Models;

namespace Api.Services;

public interface IDiscountService
{
    Task<Result<IReadOnlyList<Discount>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<Discount>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<Discount>> CreateAsync(CreateDiscountRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, UpdateDiscountRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
