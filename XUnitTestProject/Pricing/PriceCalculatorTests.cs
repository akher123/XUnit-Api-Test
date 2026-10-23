using Api.Pricing;
using Xunit.Abstractions;

namespace XUnitTestProject.Pricing;

/// <summary>
/// IClassFixture shares one PriceCalculator for every test in this class.
/// xUnit still creates a new test-class instance for each test.
/// </summary>
[Trait("Category", "Pricing")]
public class PriceCalculatorTests : IClassFixture<PriceCalculator>
{
    private readonly PriceCalculator _calculator;
    private readonly ITestOutputHelper _output;

    public PriceCalculatorTests(PriceCalculator calculator, ITestOutputHelper output)
    {
        _calculator = calculator;
        _output = output;
    }

    // [MemberData] reads rows from this property.
    public static TheoryData<decimal, decimal, decimal> BoundaryPercentages { get; } = new()
    {
        { 100m, 0.01m, 99.99m },
        { 50m, 100m, 0m }
    };

    // [Fact] runs this case once.
    [Fact]
    public void Quote_TenPercentOfOneHundred_ReturnsNinety()
    {
        // Arrange
        const decimal price = 100.00m;
        const decimal percentage = 10m;

        // Act
        var quote = _calculator.Quote(price, percentage);

        // ITestOutputHelper prints this line in the test output.
        _output.WriteLine($"Original {quote.OriginalPrice}, discount {quote.DiscountAmount}, final {quote.FinalPrice}");

        // Assert
        Assert.Equal(100.00m, quote.OriginalPrice);
        Assert.Equal(10.00m, quote.DiscountAmount);
        Assert.Equal(90.00m, quote.FinalPrice);
    }

    // [Theory] + [InlineData] runs one row per attribute.
    [Theory]
    [InlineData(50, 20, 40)]
    [InlineData(80, 25, 60)]
    [InlineData(25, 100, 0)]
    public void Quote_KnownPrices_ReturnsExpectedFinalPrice(decimal price, decimal percentage, decimal expectedFinal)
    {
        var quote = _calculator.Quote(price, percentage);

        Assert.Equal(expectedFinal, quote.FinalPrice);
    }

    [Theory]
    [MemberData(nameof(BoundaryPercentages))]
    public void Quote_BoundaryPercentages_ReturnsExpectedFinalPrice(decimal price, decimal percentage, decimal expectedFinal)
    {
        var quote = _calculator.Quote(price, percentage);

        Assert.Equal(expectedFinal, quote.FinalPrice);
    }

    // [ClassData] reads rows from RoundingQuotes.
    [Theory]
    [ClassData(typeof(RoundingQuotes))]
    public void Quote_RoundedPercentages_ReturnsExpectedFinalPrice(decimal price, decimal percentage, decimal expectedFinal)
    {
        var quote = _calculator.Quote(price, percentage);

        Assert.Equal(expectedFinal, quote.FinalPrice);
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(10, 101)]
    [InlineData(-1, 10)]
    public void Quote_InvalidPriceOrPercentage_Throws(decimal price, decimal percentage)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Quote(price, percentage));
    }
}

public class RoundingQuotes : TheoryData<decimal, decimal, decimal>
{
    public RoundingQuotes()
    {
        Add(10.00m, 33.33m, 6.67m);
        Add(1.00m, 2.5m, 0.97m);
    }
}
