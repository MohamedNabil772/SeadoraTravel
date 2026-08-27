namespace Seadora.Booking.Domain.ValueObjects;

public class Money
{
    public decimal Subtotal { get; private set; }
    public decimal AddonsTotal { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TaxTotal { get; private set; }
    public decimal Total { get; private set; }
    public string Currency { get; private set; } = "EUR";
    public decimal AmountPaid { get; private set; }
    public decimal BalanceDue { get; private set; }

    private Money() { }

    public static Money Create(decimal subtotal, decimal addonsTotal, decimal discount,
                               decimal taxTotal, string currency, decimal amountPaid = 0)
    {
        if (subtotal < 0 || addonsTotal < 0 || discount < 0 || taxTotal < 0 || amountPaid < 0)
            throw new ArgumentException("Money components cannot be negative.");
        var total = subtotal + addonsTotal - discount + taxTotal;
        if (total < 0) throw new ArgumentException("Total cannot be negative.");
        return new Money
        {
            Subtotal = subtotal,
            AddonsTotal = addonsTotal,
            Discount = discount,
            TaxTotal = taxTotal,
            Currency = string.IsNullOrWhiteSpace(currency) ? "EUR" : currency,
            Total = total,
            AmountPaid = amountPaid,
            BalanceDue = total - amountPaid
        };
    }

    public Money WithPayment(decimal amountPaid)
    {
        if (amountPaid < 0) throw new ArgumentException("AmountPaid cannot be negative.");
        return new Money
        {
            Subtotal = Subtotal,
            AddonsTotal = AddonsTotal,
            Discount = Discount,
            TaxTotal = TaxTotal,
            Currency = Currency,
            Total = Total,
            AmountPaid = amountPaid,
            BalanceDue = Total - amountPaid
        };
    }
}
