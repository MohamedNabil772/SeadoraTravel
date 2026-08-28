using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using MassTransit;

namespace Seadora.Concierge.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
