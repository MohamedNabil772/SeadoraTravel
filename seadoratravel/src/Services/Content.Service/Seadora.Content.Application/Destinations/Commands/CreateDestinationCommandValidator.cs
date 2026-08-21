using FluentValidation;

namespace Seadora.Content.Application.Destinations.Commands;

public class CreateDestinationCommandValidator : AbstractValidator<CreateDestinationCommand>
{
    public CreateDestinationCommandValidator()
    {
        RuleFor(x => x.Names)
            .Must(HaveAtLeastOneLanguage)
            .WithMessage("Names must contain at least one language entry.");
    }

    private bool HaveAtLeastOneLanguage(Dictionary<string, string> dict)
    {
        if (dict == null || dict.Count == 0) return false;
        return dict.Any(kvp => !string.IsNullOrWhiteSpace(kvp.Value));
    }
}
