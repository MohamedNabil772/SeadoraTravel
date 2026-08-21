using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Languages;

public record GetTranslationsQuery(string LanguageCode) : IRequest<Dictionary<string, string>>;

public class GetTranslationsQueryHandler : IRequestHandler<GetTranslationsQuery, Dictionary<string, string>>
{
    private readonly IContentDbContext _context;

    public GetTranslationsQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, string>> Handle(GetTranslationsQuery request, CancellationToken cancellationToken)
    {
        var translations = await _context.Translations
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new Dictionary<string, string>();
        foreach (var t in translations)
        {
            if (t.Values != null && t.Values.TryGetValue(request.LanguageCode, out var val))
            {
                result[t.Key] = val;
            }
        }

        return result;
    }
}

public record GetAllTranslationsQuery() : IRequest<List<TranslationItemDto>>;

public class TranslationItemDto
{
    public string Key { get; set; } = string.Empty;
    public string Namespace { get; set; } = "default";
    public Dictionary<string, string> Values { get; set; } = new();
}

public class GetAllTranslationsQueryHandler : IRequestHandler<GetAllTranslationsQuery, List<TranslationItemDto>>
{
    private readonly IContentDbContext _context;

    public GetAllTranslationsQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<TranslationItemDto>> Handle(GetAllTranslationsQuery request, CancellationToken cancellationToken)
    {
        var translations = await _context.Translations.AsNoTracking().ToListAsync(cancellationToken);

        return translations
            .Select(t => new TranslationItemDto
            {
                Key = t.Key,
                Namespace = string.IsNullOrEmpty(t.Namespace) ? "common" : t.Namespace,
                Values = t.Values ?? new Dictionary<string, string>()
            })
            .ToList();
    }
}
