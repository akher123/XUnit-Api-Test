using Api.Models;

namespace Api.Repositories;

public interface IDiscountRepository
{
    Task<IReadOnlyList<Discount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Discount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Discount> AddAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
