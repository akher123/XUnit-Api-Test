namespace Api.Pricing;

public sealed record PriceQuote(decimal OriginalPrice, decimal DiscountAmount, decimal FinalPrice);

public sealed class PriceCalculator
{
    public PriceQuote Quote(decimal price, decimal percentage)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, "Price cannot be negative.");
        }

        if (percentage < 0.01m || percentage > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), percentage, "Percentage must be between 0.01 and 100.");
        }

        var discountAmount = Math.Round(price * percentage / 100m, 2, MidpointRounding.AwayFromZero);
        return new PriceQuote(price, discountAmount, price - discountAmount);
    }
}
