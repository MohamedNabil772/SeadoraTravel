using Seadora.Booking.Domain.ValueObjects;

namespace Seadora.UnitTests;

public class MoneyTests
{
    [Fact]
    public void Create_Computes_Total_And_BalanceDue()
    {
        var money = Money.Create(subtotal: 100m, addonsTotal: 25m, discount: 10m, taxTotal: 5m,
            currency: "EUR", amountPaid: 40m);

        Assert.Equal(120m, money.Total); // 100 + 25 - 10 + 5
        Assert.Equal(80m, money.BalanceDue);
        Assert.Equal("EUR", money.Currency);
    }

    [Theory]
    [InlineData(-1, 0, 0, 0, 0)]
    [InlineData(0, -1, 0, 0, 0)]
    [InlineData(0, 0, -1, 0, 0)]
    [InlineData(0, 0, 0, -1, 0)]
    [InlineData(0, 0, 0, 0, -1)]
    public void Create_Throws_On_Negative_Component(decimal subtotal, decimal addons, decimal discount, decimal tax, decimal paid)
    {
        Assert.Throws<ArgumentException>(() => Money.Create(subtotal, addons, discount, tax, "EUR", paid));
    }

    [Fact]
    public void Create_Throws_When_Discount_Exceeds_Everything()
    {
        Assert.Throws<ArgumentException>(() => Money.Create(10m, 0m, 50m, 0m, "EUR"));
    }

    [Fact]
    public void WithPayment_Updates_AmountPaid_And_BalanceDue()
    {
        var money = Money.Create(200m, 0m, 0m, 0m, "EUR");

        var paid = money.WithPayment(money.Total);
        Assert.Equal(200m, paid.AmountPaid);
        Assert.Equal(0m, paid.BalanceDue);

        var unpaid = paid.WithPayment(0m);
        Assert.Equal(0m, unpaid.AmountPaid);
        Assert.Equal(200m, unpaid.BalanceDue);
    }
}
