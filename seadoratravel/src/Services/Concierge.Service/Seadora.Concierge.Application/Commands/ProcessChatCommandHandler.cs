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

        var lang = (request.Language ?? "en").ToLowerInvariant().Trim();
        if (lang.Length > 2) lang = lang.Substring(0, 2);
        if (lang != "de" && lang != "fr" && lang != "it" && lang != "ru") lang = "en";

        var intent = ConciergeIntent.General;
        var msgLower = (request.Message ?? string.Empty).ToLowerInvariant();
        if (msgLower.Contains("search") || msgLower.Contains("tour") || msgLower.Contains("trip") ||
            msgLower.Contains("excursion") || msgLower.Contains("luxor") || msgLower.Contains("cairo") ||
            msgLower.Contains("kairo") || msgLower.Contains("safari") || msgLower.Contains("dolphin") ||
            msgLower.Contains("delfin") || msgLower.Contains("island") || msgLower.Contains("insel") ||
            msgLower.Contains("boat") || msgLower.Contains("boot") || msgLower.Contains("sea") ||
            msgLower.Contains("meer") || msgLower.Contains("recommend") || msgLower.Contains("book") ||
            msgLower.Contains("tauch") || msgLower.Contains("plongée") || msgLower.Contains("дайвинг") ||
            msgLower.Contains("тур") || msgLower.Contains("экскурси") || msgLower.Contains("сафари") ||
            msgLower.Contains("остров") || msgLower.Contains("каир") || msgLower.Contains("луксор"))
        {
            intent = ConciergeIntent.TourSearch;
        }
        else if (msgLower.Contains("cancel") || msgLower.Contains("stornier") || msgLower.Contains("annul") || msgLower.Contains("отмен"))
        {
            intent = ConciergeIntent.CancellationPolicy;
        }

        var defaultGreetings = new Dictionary<string, string>
        {
            { "en", "How can I help you with your luxury Red Sea experience today?" },
            { "de", "Wie kann ich Ihnen heute bei Ihrem Luxus-Erlebnis am Roten Meer helfen?" },
            { "fr", "Comment puis-je vous aider aujourd'hui pour votre expérience de luxe en mer Rouge ?" },
            { "it", "Come posso aiutarvi oggi con la vostra esperienza di lusso sul Mar Rosso?" },
            { "ru", "Чем я могу помочь вам сегодня с вашим роскошным отдыхом на Красном море?" }
        };

        var foundMessages = new Dictionary<string, string>
        {
            { "en", "I found some exceptional excursions tailored for you:" },
            { "de", "Ich habe einige außergewöhnliche Ausflüge für Sie ausgewählt:" },
            { "fr", "Voici quelques excursions exceptionnelles sélectionnées pour vous :" },
            { "it", "Ecco alcune straordinarie escursioni selezionate per voi:" },
            { "ru", "Я подобрал для вас отличные экскурсии:" }
        };

        var notFoundMessages = new Dictionary<string, string>
        {
            { "en", "I couldn't find any matching tours at this moment. Feel free to ask about our sea trips, desert safaris, or Cairo & Luxor excursions." },
            { "de", "Ich konnte im Moment keine passenden Touren finden. Fragen Sie gerne nach unseren Bootstouren, Wüstensafaris oder Ausflügen nach Kairo und Luxor." },
            { "fr", "Je n'ai trouvé aucune excursion correspondante pour le moment. N'hésitez pas à demander nos sorties en mer, safaris ou visites du Caire et Louxor." },
            { "it", "Al momento non ho trovato tour corrispondenti. Chiedete pure delle nostre gite in barca, safari nel deserto o escursioni al Cairo e Luxor." },
            { "ru", "К сожалению, по вашему запросу ничего не найдено. Спросите меня о морских прогулках, сафари в пустыне или экскурсиях в Каир и Луксор." }
        };

        var cancellationMessages = new Dictionary<string, string>
        {
            { "en", "Our cancellation policy allows free cancellation up to 24-48 hours before the tour for a 100% full refund." },
            { "de", "Unsere Stornierungsbedingungen erlauben eine kostenlose Stornierung bis zu 24-48 Stunden vor Beginn der Tour mit 100% Rückerstattung." },
            { "fr", "Notre politique d'annulation permet une annulation gratuite jusqu'à 24-48 heures avant l'excursion avec un remboursement intégral à 100%." },
            { "it", "La nostra politica consente la cancellazione gratuita fino a 24-48 ore prima del tour con rimborso completo al 100%." },
            { "ru", "Вы можете бесплатно отменить бронирование за 24–48 часов до начала экскурсии со 100% возвратом средств." }
        };

        var responseContent = defaultGreetings.GetValueOrDefault(lang, defaultGreetings["en"]);
        var suggestedTours = new List<SuggestedTour>();

        if (intent == ConciergeIntent.TourSearch)
        {
            var query = _dbContext.TourCatalogIndices
                .Where(t => (request.BranchId == Guid.Empty || t.BranchId == request.BranchId) && t.IsActive);

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "what", "have", "with", "want", "like", "trip", "trips", "tour", "tours", "book", "show", "recommend", "please", "some", "from", "can", "you", "the",
                "was", "wie", "ich", "möchte", "bitte", "zeigen", "touren", "ausflug", "ausflüge", "haben",
                "que", "pour", "les", "des", "une", "dans", "avec", "est",
                "cosa", "per", "con", "sono", "vorrei", "mostra",
                "что", "как", "есть", "хочу", "для", "покажи", "туры", "экскурсии"
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
                        var namesLower = t.Names?.ToLowerInvariant() ?? string.Empty;

                        foreach (var w in words)
                        {
                            if (titleLower.Contains(w)) score += 10;
                            if (namesLower.Contains(w)) score += 10;
                            if (catLower.Contains(w)) score += 5;
                            if (destLower.Contains(w)) score += 5;
                            if (descLower.Contains(w)) score += 2;
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
                responseContent = foundMessages.GetValueOrDefault(lang, foundMessages["en"]);
                suggestedTours = matchingTours.Select(t =>
                {
                    Dictionary<string, string>? namesDict = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(t.Names))
                        {
                            namesDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(t.Names);
                        }
                    }
                    catch { }

                    var tourTitle = t.Title;
                    if (namesDict != null && namesDict.TryGetValue(lang, out var localizedTitle) && !string.IsNullOrWhiteSpace(localizedTitle))
                    {
                        tourTitle = localizedTitle;
                    }

                    return new SuggestedTour
                    {
                        TourId = t.TourId,
                        Slug = t.Slug,
                        Title = tourTitle,
                        Names = namesDict,
                        PriceEur = t.PriceEur,
                        Rating = t.Rating,
                        MainImage = t.MainImage
                    };
                }).ToList();
            }
            else
            {
                responseContent = notFoundMessages.GetValueOrDefault(lang, notFoundMessages["en"]);
            }
        }
        else if (intent == ConciergeIntent.CancellationPolicy)
        {
            responseContent = cancellationMessages.GetValueOrDefault(lang, cancellationMessages["en"]);
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
