using MediatR;
using System;
using System.Collections.Generic;

namespace Seadora.Concierge.Application.Commands;

public class ProcessChatCommand : IRequest<ProcessChatResponse>
{
    public Guid SessionId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? VisitorId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ProcessChatResponse
{
    public Guid MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public List<SuggestedTour> SuggestedTours { get; set; } = new();
    public List<string> QuickReplies { get; set; } = new();
}

public class SuggestedTour
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal PriceEur { get; set; }
    public decimal? Rating { get; set; }
    public string? MainImage { get; set; }
}
