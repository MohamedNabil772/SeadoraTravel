using Seadora.Finance.Domain.Enums;

namespace Seadora.Finance.Domain.Entities;

public class LedgerAccount
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public NormalSide NormalSide { get; set; }
}
