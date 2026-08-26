using System.Text.Json;
using Seadora.Contracts.Messaging;

namespace Seadora.Common.Messaging.Outbox;

public sealed class OutboxWriter(IOutboxDbContext ctx) : IOutboxWriter
{
    // ponytail: no SaveChanges here — the caller's domain transaction owns the commit.
    public void Enqueue(IIntegrationEvent evt) =>
        ctx.OutboxMessages.Add(new OutboxMessage
        {
            Id = evt.Id,
            Type = evt.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(evt, evt.GetType()),
            OccurredUtc = evt.OccurredUtc
        });
}
