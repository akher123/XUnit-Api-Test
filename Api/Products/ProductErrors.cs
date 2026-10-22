using Api;

namespace Api.Products;

public static class ProductErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Product.NotFound", $"Product with id {id} was not found.");
}
