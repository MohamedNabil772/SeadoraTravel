using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.DTOs;
using Mapster;

namespace Seadora.Booking.Application.Feedbacks.Commands.CreateFeedback;

public record CreateFeedbackCommand(
    Guid TourId,
    double Rating,
    string Comment,
    string CustomerName,
    string CustomerEmail
) : IRequest<FeedbackDto>;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, FeedbackDto>
{
    private readonly IBookingDbContext _context;

    public CreateFeedbackCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<FeedbackDto> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating < 0.5 || request.Rating > 5.0)
        {
            throw new ArgumentException("Rating must be between 1 and 5.");
        }

        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            TourId = request.TourId,
            Rating = request.Rating,
            Comment = request.Comment ?? string.Empty,
            CustomerName = request.CustomerName ?? string.Empty,
            CustomerEmail = request.CustomerEmail ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync(cancellationToken);

        return feedback.Adapt<FeedbackDto>();
    }
}
