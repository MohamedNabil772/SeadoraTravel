using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Domain.Enums;
using System;

namespace Seadora.Booking.Domain.Services.Refunds;

public interface IRefundProcessor
{
    decimal CalculateRefund(Entities.Booking booking, decimal totalCost, DateTime cancellationTime);
}

public class CashRefundProcessor : IRefundProcessor
{
    public decimal CalculateRefund(Entities.Booking booking, decimal totalCost, DateTime cancellationTime)
    {
        // For cash bookings, typically no refund is given as no payment was made upfront,
        // or a different policy applies. Here we return 0 for cash.
        return 0m;
    }
}

public class OnlineRefundProcessor : IRefundProcessor
{
    private readonly ICancellationPolicyService _cancellationPolicyService;

    public OnlineRefundProcessor(ICancellationPolicyService cancellationPolicyService)
    {
        _cancellationPolicyService = cancellationPolicyService;
    }

    public decimal CalculateRefund(Entities.Booking booking, decimal totalCost, DateTime cancellationTime)
    {
        return _cancellationPolicyService.CalculateRefundAmount(booking, totalCost, cancellationTime);
    }
}

public interface IRefundProcessorFactory
{
    IRefundProcessor GetRefundProcessor(bool isPaid); // Assuming isPaid true means online payment
}

public class RefundProcessorFactory : IRefundProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RefundProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRefundProcessor GetRefundProcessor(bool isPaid)
    {
        if (isPaid)
        {
            return (IRefundProcessor)_serviceProvider.GetService(typeof(OnlineRefundProcessor))!;
        }
        else
        {
            return (IRefundProcessor)_serviceProvider.GetService(typeof(CashRefundProcessor))!;
        }
    }
}
