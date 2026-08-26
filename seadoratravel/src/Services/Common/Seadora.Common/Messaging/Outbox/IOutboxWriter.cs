namespace Seadora.Common.Messaging.Outbox;

public interface IOutboxWriter
{
    void Enqueue(IIntegrationEvent evt);
}
