using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Feedbacks.Queries.GetFeedbacks;

public record GetFeedbacksQuery(Guid? TourId, bool IncludeHidden = false) : IRequest<List<Feedback>>;

public class GetFeedbacksQueryHandler : IRequestHandler<GetFeedbacksQuery, List<Feedback>>
{
    private readonly IBookingDbContext _context;

    public GetFeedbacksQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Feedback>> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Feedbacks.AsQueryable();

        if (!request.IncludeHidden)
        {
            query = query.Where(f => f.IsVisible);
        }

        if (request.TourId.HasValue)
        {
            query = query.Where(f => f.TourId == request.TourId.Value);
        }

        return await query.OrderByDescending(f => f.CreatedAt).ToListAsync(cancellationToken);
    }
}
