using Api.Discounts;
using Api.DTOs;
using Api.Models;
using Api.Products;
using Api.Repositories;

namespace Api.Services;

public class DiscountService(IDiscountRepository repository, IProductRepository productRepository) : IDiscountService
{
    public async Task<Result<IReadOnlyList<Discount>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var discounts = await repository.GetAllAsync(cancellationToken);
        return Result.Success(discounts);
    }

    public async Task<Result<Discount>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var discount = await repository.GetByIdAsync(id, cancellationToken);
        if (discount is null)
        {
            return Result.Failure<Discount>(DiscountErrors.NotFound(id));
        }

        return discount;
    }

    public async Task<Result<Discount>> CreateAsync(CreateDiscountRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure<Discount>(ProductErrors.NotFound(request.ProductId));
        }

        var discount = new Discount
        {
            ProductId = request.ProductId,
            Name = request.Name,
            Percentage = request.Percentage
        };

        return await repository.AddAsync(discount, cancellationToken);
    }

    public async Task<Result> UpdateAsync(int id, UpdateDiscountRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return Result.Failure(DiscountErrors.NotFound(id));
        }

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        existing.ProductId = request.ProductId;
        existing.Name = request.Name;
        existing.Percentage = request.Percentage;

        var updated = await repository.UpdateAsync(existing, cancellationToken);
        if (!updated)
        {
            return Result.Failure(DiscountErrors.NotFound(id));
        }

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Result.Failure(DiscountErrors.NotFound(id));
        }

        return Result.Success();
    }
}
