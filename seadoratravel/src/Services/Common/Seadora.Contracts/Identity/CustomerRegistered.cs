using System;
using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Identity;

public record CustomerRegistered(string UserId, string Email, string FirstName, string LastName, string BranchId) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
