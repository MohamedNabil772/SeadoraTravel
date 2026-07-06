using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Feedbacks.Commands.UpdateFeedbackVisibility;

public record UpdateFeedbackVisibilityCommand(Guid Id, bool IsVisible) : IRequest<Unit>;

public class UpdateFeedbackVisibilityCommandHandler : IRequestHandler<UpdateFeedbackVisibilityCommand, Unit>
{
    private readonly IBookingDbContext _context;

    public UpdateFeedbackVisibilityCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateFeedbackVisibilityCommand request, CancellationToken cancellationToken)
    {
        var feedback = await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (feedback == null)
        {
            throw new KeyNotFoundException("Feedback not found.");
        }

        feedback.IsVisible = request.IsVisible;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
