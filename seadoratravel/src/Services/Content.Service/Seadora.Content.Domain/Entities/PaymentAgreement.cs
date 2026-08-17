using System;

namespace Seadora.Content.Domain.Entities;

public class PaymentAgreement
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
