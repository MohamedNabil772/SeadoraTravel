using MediatR;

namespace Seadora.Content.Application.Languages;

public record BulkUpdateTranslationsCommand(string LanguageCode, Dictionary<string, string> Translations) : IRequest<bool>;

public class BulkUpdateTranslationsCommandHandler : IRequestHandler<BulkUpdateTranslationsCommand, bool>
{
    public Task<bool> Handle(BulkUpdateTranslationsCommand request, CancellationToken cancellationToken)
    {
        // Mock implementation
        return Task.FromResult(true);
    }
}
