using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace Seadora.Booking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(Seadora.Common.Behaviors.ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<Seadora.Booking.Domain.Services.ICancellationPolicyService, Seadora.Booking.Domain.Services.CancellationPolicyService>();
        services.AddScoped<Seadora.Booking.Domain.Services.Refunds.CashRefundProcessor>();
        services.AddScoped<Seadora.Booking.Domain.Services.Refunds.OnlineRefundProcessor>();
        services.AddScoped<Seadora.Booking.Domain.Services.Refunds.IRefundProcessorFactory, Seadora.Booking.Domain.Services.Refunds.RefundProcessorFactory>();
        return services;
    }
}
