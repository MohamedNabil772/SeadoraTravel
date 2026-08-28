using MediatR;
using Seadora.Concierge.Domain.Entities;
using Seadora.Concierge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Seadora.Concierge.Application.Commands;

public interface IConciergeDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<ConversationSession> ConversationSessions { get; }
    Microsoft.EntityFrameworkCore.DbSet<ConversationMessage> ConversationMessages { get; }
    Microsoft.EntityFrameworkCore.DbSet<TourCatalogIndex> TourCatalogIndices { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class ProcessChatCommandHandler : IRequestHandler<ProcessChatCommand, ProcessChatResponse>
{
    private readonly IConciergeDbContext _dbContext;

    public ProcessChatCommandHandler(IConciergeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProcessChatResponse> Handle(ProcessChatCommand request, CancellationToken cancellationToken)
    {
        var session = await _dbContext.ConversationSessions
            .FindAsync(new object[] { request.SessionId }, cancellationToken);

        if (session == null)
        {
            session = new ConversationSession
            {
                Id = request.SessionId,
                BranchId = request.BranchId,
                VisitorId = request.VisitorId,
                CreatedUtc = DateTime.UtcNow,
                LastActiveUtc = DateTime.UtcNow,
                Messages = new List<ConversationMessage>()
            };
            _dbContext.ConversationSessions.Add(session);
        }

        session.LastActiveUtc = DateTime.UtcNow;

        var userMessage = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            SessionId = request.SessionId,
            Role = "User",
            Content = request.Message,
            CreatedUtc = DateTime.UtcNow
        };
        _dbContext.ConversationMessages.Add(userMessage);

        var intent = ConciergeIntent.General;
        if (request.Message.Contains("search", StringComparison.OrdinalIgnoreCase) || request.Message.Contains("tour", StringComparison.OrdinalIgnoreCase))
        {
            intent = ConciergeIntent.TourSearch;
        }
        else if (request.Message.Contains("cancel", StringComparison.OrdinalIgnoreCase))
        {
            intent = ConciergeIntent.CancellationPolicy;
        }

        var responseContent = "How can I help you today?";
        var suggestedTours = new List<SuggestedTour>();

        if (intent == ConciergeIntent.TourSearch)
        {
            var tours = _dbContext.TourCatalogIndices
                .Where(t => t.BranchId == request.BranchId && t.IsActive)
                .Take(3)
                .ToList();
            
            if (tours.Any())
            {
                responseContent = "I found some tours for you.";
                suggestedTours = tours.Select(t => new SuggestedTour
                {
                    TourId = t.TourId,
                    Title = t.Title,
                    PriceEur = t.PriceEur,
                    Rating = t.Rating,
                    MainImage = t.MainImage
                }).ToList();
            }
            else
            {
                responseContent = "I couldn't find any matching tours.";
            }
        }
        else if (intent == ConciergeIntent.CancellationPolicy)
        {
            responseContent = "Our cancellation policy allows free cancellation up to 48 hours before the tour.";
        }

        var botMessage = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            SessionId = request.SessionId,
            Role = "Assistant",
            Content = responseContent,
            Intent = intent,
            SuggestedTourIds = suggestedTours.Select(t => t.TourId).ToList(),
            CreatedUtc = DateTime.UtcNow
        };
        _dbContext.ConversationMessages.Add(botMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProcessChatResponse
        {
            MessageId = botMessage.Id,
            Content = botMessage.Content,
            Intent = botMessage.Intent.ToString()!,
            SuggestedTours = suggestedTours,
            QuickReplies = new List<string> { "Contact Support", "Check Availability" }
        };
    }
}
