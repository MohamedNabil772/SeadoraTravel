using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Seadora.Content.Application.Concierge;

namespace Seadora.Content.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(Seadora.Common.Behaviors.ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddHttpClient<IConciergeService, ConciergeService>(client => 
        {
            client.BaseAddress = new System.Uri("http://booking-service:8080/");
        });
        
        return services;
    }
}
