using MediatR;
using System;

namespace Seadora.Booking.Application.Inquiries.Commands.ReplyToContactInquiry;

public record ReplyToContactInquiryCommand(
    Guid Id,
    string ReplyMessage
) : IRequest;
