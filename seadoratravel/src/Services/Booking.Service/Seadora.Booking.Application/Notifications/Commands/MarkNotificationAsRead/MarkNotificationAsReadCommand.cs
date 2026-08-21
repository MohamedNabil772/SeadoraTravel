using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
{
    private readonly IBookingDbContext _context;

    public MarkNotificationAsReadCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (notification == null)
            throw new KeyNotFoundException($"Notification with ID {request.NotificationId} not found.");

        notification.MarkAsRead();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
