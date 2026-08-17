using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Domain.Services;

/// <summary>
/// DRAFT STATE: This service calculates penalties based on the pending business rules.
/// Do not wire into MediatR handlers until rules are officially activated.
/// </summary>
public interface ICancellationPolicyService
{
    decimal CalculateRefundAmount(Entities.Booking booking, decimal totalCost, DateTime cancellationRequestTime);
    bool IsCashReservationValid(Entities.Booking booking, DateTime currentTime);
}

public class CancellationPolicyService : ICancellationPolicyService
{
    public decimal CalculateRefundAmount(Entities.Booking booking, decimal totalCost, DateTime cancellationRequestTime)
    {
        var hoursUntilTour = (booking.BookingDate - cancellationRequestTime).TotalHours;

        if (hoursUntilTour >= 72)
        {
            // Free cancellation
            return totalCost;
        }
        else if (hoursUntilTour >= 48)
        {
            // 25% penalty
            return totalCost * 0.75m;
        }
        else if (hoursUntilTour < 24)
        {
            // 50% penalty
            return totalCost * 0.50m;
        }
        
        // Default fallback
        return totalCost;
    }

    public bool IsCashReservationValid(Entities.Booking booking, DateTime currentTime)
    {
        // If the booking is marked as "Cash" and is still "Pending", 
        // it becomes invalid (should be cancelled) if we are within 48 hours of the tour.
        var hoursUntilTour = (booking.BookingDate - currentTime).TotalHours;
        
        if (booking.Status == Seadora.Booking.Domain.Enums.BookingStatus.Pending && hoursUntilTour <= 48)
        {
            return false; // Invalid, trigger cancellation
        }

        return true;
    }
}
