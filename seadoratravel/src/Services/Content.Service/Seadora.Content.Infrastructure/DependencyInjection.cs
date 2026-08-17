using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Infrastructure.Persistence;

namespace Seadora.Content.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ContentDbContext>(options =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
            
        services.AddScoped<IContentDbContext>(provider => provider.GetRequiredService<ContentDbContext>());
        
        services.AddScoped<IExcelLocalizationService, Seadora.Content.Infrastructure.Services.ExcelLocalizationService>();
        services.AddScoped<IQuestPdfGeneratorService, Seadora.Content.Infrastructure.Services.QuestPdfGeneratorService>();
        
        return services;
    }
}
