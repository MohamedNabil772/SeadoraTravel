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
        // Proximity is measured against the tour date, not BookingDate (the record-creation stamp).
        if (booking.TourDate is null)
        {
            return totalCost; // no tour date on record -> nothing to penalise against
        }

        var hoursUntilTour = (booking.TourDate.Value - cancellationRequestTime).TotalHours;

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
        // Proximity is measured against the tour date, not BookingDate (the record-creation stamp).
        if (booking.TourDate is null)
        {
            return true; // no tour date on record -> never auto-cancel
        }

        var hoursUntilTour = (booking.TourDate.Value - currentTime).TotalHours;
        
        if (booking.Status == Seadora.Booking.Domain.Enums.BookingStatus.Pending && hoursUntilTour <= 48)
        {
            return false; // Invalid, trigger cancellation
        }

        return true;
    }
}
