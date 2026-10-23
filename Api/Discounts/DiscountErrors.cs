using Api;

namespace Api.Discounts;

public static class DiscountErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Discount.NotFound", $"Discount with id {id} was not found.");
}
