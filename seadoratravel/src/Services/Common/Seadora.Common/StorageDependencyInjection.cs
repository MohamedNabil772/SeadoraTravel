using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Common.Storage;

namespace Seadora.Common;

public static class StorageDependencyInjection
{
    public static IServiceCollection AddSeadoraStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var storageType = configuration["StorageSettings:Type"] ?? "Local";

        if (storageType.Equals("Remote", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHttpClient<IStorageService, RemoteStorageService>(client =>
            {
                client.BaseAddress = new Uri(configuration["StorageSettings:RemoteUrl"] ?? "http://file-server");
            });
        }
        else
        {
            services.AddSingleton<IStorageService>(sp => 
                new LocalStorageService(configuration["StorageSettings:LocalPath"] ?? "uploads"));
        }

        return services;
    }
}
