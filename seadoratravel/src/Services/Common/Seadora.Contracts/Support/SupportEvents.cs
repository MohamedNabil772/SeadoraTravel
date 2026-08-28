using System;
using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Support;

public record InquiryReceived(Guid InquiryId, string CustomerName, string CustomerEmail, string Subject, string Body, DateTime ReceivedAt) : IntegrationEvent;

public record TicketCreated(Guid TicketId, string CustomerEmail, string Subject) : IntegrationEvent;

public record TicketReplied(Guid TicketId, Guid MessageId, string Body, bool IsFromAgent) : IntegrationEvent;

public record TicketStatusChanged(Guid TicketId, int OldStatus, int NewStatus) : IntegrationEvent;
