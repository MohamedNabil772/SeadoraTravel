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
        var sessionId = request.SessionId == Guid.Empty ? Guid.NewGuid() : request.SessionId;
        var session = await _dbContext.ConversationSessions
            .FindAsync(new object[] { sessionId }, cancellationToken);

        if (session == null)
        {
            session = new ConversationSession
            {
                Id = sessionId,
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
            SessionId = session.Id,
            Role = "User",
            Content = request.Message,
            CreatedUtc = DateTime.UtcNow
        };
        _dbContext.ConversationMessages.Add(userMessage);

        var intent = ConciergeIntent.General;
        var msgLower = (request.Message ?? string.Empty).ToLowerInvariant();
        if (msgLower.Contains("search") || msgLower.Contains("tour") || msgLower.Contains("trip") ||
            msgLower.Contains("excursion") || msgLower.Contains("luxor") || msgLower.Contains("cairo") ||
            msgLower.Contains("safari") || msgLower.Contains("dolphin") || msgLower.Contains("island") ||
            msgLower.Contains("boat") || msgLower.Contains("sea") || msgLower.Contains("recommend") ||
            msgLower.Contains("book"))
        {
            intent = ConciergeIntent.TourSearch;
        }
        else if (msgLower.Contains("cancel"))
        {
            intent = ConciergeIntent.CancellationPolicy;
        }

        var responseContent = "How can I help you today?";
        var suggestedTours = new List<SuggestedTour>();

        if (intent == ConciergeIntent.TourSearch)
        {
            var query = _dbContext.TourCatalogIndices
                .Where(t => (request.BranchId == Guid.Empty || t.BranchId == request.BranchId) && t.IsActive);

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "what", "have", "with", "want", "like", "trip", "trips", "tour", "tours", "book", "show", "recommend", "please", "some", "from", "can", "you", "the"
            };

            var words = msgLower.Split(new[] { ' ', ',', '.', '?', '!', ';', ':', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2 && !stopWords.Contains(w))
                .Select(w => (w.EndsWith("s") && w.Length > 3) ? w.Substring(0, w.Length - 1) : w)
                .ToList();

            var activeTours = query.ToList();
            List<TourCatalogIndex> matchingTours = new();

            if (words.Any())
            {
                matchingTours = activeTours
                    .Select(t =>
                    {
                        int score = 0;
                        var titleLower = t.Title?.ToLowerInvariant() ?? string.Empty;
                        var destLower = t.DestinationName?.ToLowerInvariant() ?? string.Empty;
                        var catLower = t.CategoryName?.ToLowerInvariant() ?? string.Empty;
                        var descLower = t.Descriptions?.ToLowerInvariant() ?? string.Empty;

                        foreach (var w in words)
                        {
                            if (titleLower.Contains(w)) score += 10;
                            if (catLower.Contains(w)) score += 5;
                            if (destLower.Contains(w)) score += 5;
                            if (descLower.Contains(w)) score += 1;
                        }
                        return new { Tour = t, Score = score };
                    })
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Tour)
                    .Take(4)
                    .ToList();
            }

            if (!matchingTours.Any())
            {
                matchingTours = activeTours.Take(3).ToList();
            }

            if (matchingTours.Any())
            {
                responseContent = "I found some exceptional excursions tailored for you:";
                suggestedTours = matchingTours.Select(t => new SuggestedTour
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
                responseContent = "I couldn't find any matching tours at this moment.";
            }
        }
        else if (intent == ConciergeIntent.CancellationPolicy)
        {
            responseContent = "Our cancellation policy allows free cancellation up to 48 hours before the tour.";
        }

        var botMessage = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
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
