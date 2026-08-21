using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.DTOs;

namespace Seadora.Booking.Application.Notifications.Queries.GetNotifications;

public record NotificationsResponseDto(List<NotificationDto> Notifications, int UnreadCount);

public record GetNotificationsQuery(int Limit = 50) : IRequest<NotificationsResponseDto>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, NotificationsResponseDto>
{
    private readonly IBookingDbContext _context;

    public GetNotificationsQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationsResponseDto> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var unreadCount = await _context.Notifications
            .Where(n => !n.IsRead)
            .CountAsync(cancellationToken);

        var notifications = await _context.Notifications
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .Take(request.Limit)
            .Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Message,
                n.Type,
                n.ReferenceId,
                n.MetadataJson,
                n.IsRead,
                n.CreatedAt,
                n.ReadAt
            ))
            .ToListAsync(cancellationToken);

        return new NotificationsResponseDto(notifications, unreadCount);
    }
}
