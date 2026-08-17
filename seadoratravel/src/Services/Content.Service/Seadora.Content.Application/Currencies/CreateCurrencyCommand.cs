using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Currencies;

public record CreateCurrencyCommand(string Code, string Name, string Symbol, decimal ExchangeRate, bool IsActive) : IRequest<Guid>;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateCurrencyCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = new Currency
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Symbol = request.Symbol,
            ExchangeRate = request.ExchangeRate,
            IsActive = request.IsActive
        };

        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync(cancellationToken);

        return currency.Id;
    }
}
