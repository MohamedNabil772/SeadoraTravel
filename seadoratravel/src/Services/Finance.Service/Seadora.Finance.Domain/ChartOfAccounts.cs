using Seadora.Finance.Domain.Enums;

namespace Seadora.Finance.Domain;

/// <summary>Fixed chart-of-accounts ids so seeding is idempotent and posting code can name accounts.</summary>
public static class ChartOfAccounts
{
    public static readonly Guid Revenue = Guid.Parse("a0000000-0000-0000-0000-000000004000");
    public static readonly Guid Discounts = Guid.Parse("a0000000-0000-0000-0000-000000004100");
    public static readonly Guid SupplierCostExpense = Guid.Parse("a0000000-0000-0000-0000-000000005000");
    public static readonly Guid Refunds = Guid.Parse("a0000000-0000-0000-0000-000000005100");
    public static readonly Guid CashBank = Guid.Parse("a0000000-0000-0000-0000-000000001000");
    public static readonly Guid AccountsReceivable = Guid.Parse("a0000000-0000-0000-0000-000000001100");
    public static readonly Guid SupplierPayable = Guid.Parse("a0000000-0000-0000-0000-000000002000");
    public static readonly Guid TaxPayable = Guid.Parse("a0000000-0000-0000-0000-000000002100");

    public static readonly (Guid Id, string Code, string Name, AccountType Type, NormalSide NormalSide)[] All =
    {
        (CashBank, "1000", "Cash/Bank", AccountType.Asset, NormalSide.Debit),
        (AccountsReceivable, "1100", "AccountsReceivable", AccountType.Asset, NormalSide.Debit),
        (SupplierPayable, "2000", "SupplierPayable", AccountType.Liability, NormalSide.Credit),
        (TaxPayable, "2100", "TaxPayable", AccountType.Liability, NormalSide.Credit),
        (Revenue, "4000", "Revenue", AccountType.Income, NormalSide.Credit),
        (Discounts, "4100", "Discounts", AccountType.Income, NormalSide.Debit),
        (SupplierCostExpense, "5000", "SupplierCostExpense", AccountType.Expense, NormalSide.Debit),
        (Refunds, "5100", "Refunds", AccountType.Expense, NormalSide.Debit)
    };
}
