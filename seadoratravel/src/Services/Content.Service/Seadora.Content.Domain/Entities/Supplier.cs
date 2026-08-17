using System;

namespace Seadora.Content.Domain.Entities;

public class Supplier
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string BankAccountInfo { get; set; } = string.Empty;
    
    public Guid PaymentAgreementId { get; set; }
    public PaymentAgreement? PaymentAgreement { get; set; }
}
