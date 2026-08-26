using Microsoft.Extensions.DependencyInjection;

namespace Seadora.Common.Tenancy;

public static class TenancyDependencyInjection
{
    // ponytail: registers ICurrentBranch only. The host API must call AddHttpContextAccessor()
    // itself when it actually consumes ICurrentBranch (that extension lives in the full
    // Microsoft.AspNetCore.Http package, not in Http.Abstractions).
    public static IServiceCollection AddSeadoraTenancy(this IServiceCollection services)
    {
        services.AddScoped<ICurrentBranch, CurrentBranchAccessor>();
        return services;
    }
}
