using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Languages;

public record BulkUpdateTranslationsCommand(string LanguageCode, Dictionary<string, string> Translations) : IRequest<bool>;

public class BulkUpdateTranslationsCommandHandler : IRequestHandler<BulkUpdateTranslationsCommand, bool>
{
    private readonly IContentDbContext _context;

    public BulkUpdateTranslationsCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(BulkUpdateTranslationsCommand request, CancellationToken cancellationToken)
    {
        var existingTranslations = await _context.Translations.ToListAsync(cancellationToken);

        foreach (var (key, value) in request.Translations)
        {
            var existing = existingTranslations.FirstOrDefault(t => t.Key == key);
            if (existing != null)
            {
                existing.Values ??= new Dictionary<string, string>();
                existing.Values[request.LanguageCode] = value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Translations.Add(new Translation
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Namespace = "common",
                    Values = new Dictionary<string, string> { [request.LanguageCode] = value },
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record BulkUpdateAdminTranslationsCommand(List<TranslationItemDto> Translations) : IRequest<bool>;

public class BulkUpdateAdminTranslationsCommandHandler : IRequestHandler<BulkUpdateAdminTranslationsCommand, bool>
{
    private readonly IContentDbContext _context;

    public BulkUpdateAdminTranslationsCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(BulkUpdateAdminTranslationsCommand request, CancellationToken cancellationToken)
    {
        var existingTranslations = await _context.Translations.ToListAsync(cancellationToken);

        foreach (var item in request.Translations)
        {
            var existing = existingTranslations.FirstOrDefault(t => t.Key == item.Key && t.Namespace == item.Namespace);
            
            if (existing != null)
            {
                existing.Values = item.Values ?? new Dictionary<string, string>();
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Translations.Add(new Translation
                {
                    Id = Guid.NewGuid(),
                    Key = item.Key,
                    Namespace = string.IsNullOrEmpty(item.Namespace) ? "common" : item.Namespace,
                    Values = item.Values ?? new Dictionary<string, string>(),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
