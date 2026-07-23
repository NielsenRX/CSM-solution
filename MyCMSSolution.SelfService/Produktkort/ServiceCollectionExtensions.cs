using Microsoft.Extensions.DependencyInjection;

namespace MyCMSSolution.SelfService.Produktkort;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProduktkortDataProvider(this IServiceCollection services)
    {
        services.AddSingleton<IProduktkortDataProvider, PlaceholderProduktkortDataProvider>();
        return services;
    }
}
