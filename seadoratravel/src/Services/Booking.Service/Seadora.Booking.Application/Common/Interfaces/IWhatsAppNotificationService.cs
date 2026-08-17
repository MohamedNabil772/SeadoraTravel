using System.Threading;
using System.Threading.Tasks;

namespace Seadora.Booking.Application.Common.Interfaces;

public interface IWhatsAppNotificationService
{
    Task<bool> SendBookingConfirmationAsync(Domain.Entities.Booking booking, CancellationToken cancellationToken = default);
    Task<bool> SendCustomMessageAsync(string toWhatsApp, string message, CancellationToken cancellationToken = default);
}
