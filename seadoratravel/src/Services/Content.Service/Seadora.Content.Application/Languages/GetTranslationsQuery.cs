using MediatR;

namespace Seadora.Content.Application.Languages;

public record GetTranslationsQuery(string LanguageCode) : IRequest<Dictionary<string, string>>;

public class GetTranslationsQueryHandler : IRequestHandler<GetTranslationsQuery, Dictionary<string, string>>
{
    public Task<Dictionary<string, string>> Handle(GetTranslationsQuery request, CancellationToken cancellationToken)
    {
        // Mock implementation since there is no Translation entity
        return Task.FromResult(new Dictionary<string, string>());
    }
}
