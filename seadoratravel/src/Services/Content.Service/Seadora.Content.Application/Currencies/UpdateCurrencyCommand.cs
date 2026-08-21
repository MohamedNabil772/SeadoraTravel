using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Currencies;

public record UpdateCurrencyCommand(Guid Id, string Code, string Name, string Symbol, decimal ExchangeRate, bool IsActive) : IRequest<bool>;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, bool>
{
    private readonly IContentDbContext _context;

    public UpdateCurrencyCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies.FindAsync(new object[] { request.Id }, cancellationToken);

        if (currency == null)
            return false;

        currency.Code = request.Code;
        currency.Name = request.Name;
        currency.Symbol = request.Symbol;
        currency.ExchangeRate = request.ExchangeRate;
        currency.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
