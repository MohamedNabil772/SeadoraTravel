using FluentValidation;

namespace Seadora.Content.Application.Destinations.Commands;

public class UpdateDestinationCommandValidator : AbstractValidator<UpdateDestinationCommand>
{
    public UpdateDestinationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Names)
            .Must(HaveAtLeastOneLanguage)
            .WithMessage("Names must contain at least one language entry.");
    }

    private bool HaveAtLeastOneLanguage(Dictionary<string, string>? dict)
    {
        if (dict == null || dict.Count == 0) return false;
        return dict.Any(kvp => !string.IsNullOrWhiteSpace(kvp.Value));
    }
}
