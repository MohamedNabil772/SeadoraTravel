using FluentValidation;

namespace Seadora.Content.Application.Concierge.Queries.ProcessChat;

public class ProcessConciergeChatQueryValidator : AbstractValidator<ProcessConciergeChatQuery>
{
    public ProcessConciergeChatQueryValidator()
    {
        RuleFor(x => x.Request).NotNull().WithMessage("Request cannot be null.");
        RuleFor(x => x.Request.Message).NotEmpty().WithMessage("Message cannot be empty.");
    }
}
