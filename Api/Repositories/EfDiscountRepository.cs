using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class EfDiscountRepository(ProductDbContext context) : IDiscountRepository
{
    public async Task<IReadOnlyList<Discount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Discounts
            .AsNoTracking()
            .OrderBy(d => d.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Discount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Discounts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Discount> AddAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        context.Discounts.Add(discount);
        await context.SaveChangesAsync(cancellationToken);
        return discount;
    }

    public async Task<bool> UpdateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        var exists = await context.Discounts.AnyAsync(d => d.Id == discount.Id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        context.Discounts.Update(discount);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var discount = await context.Discounts.FindAsync([id], cancellationToken);
        if (discount is null)
        {
            return false;
        }

        context.Discounts.Remove(discount);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
