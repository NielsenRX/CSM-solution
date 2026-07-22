using Microsoft.Extensions.DependencyInjection;

namespace MyCMSSolution.Crm.Client;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registrerer CRM-integrationslaget. Peger i dag på <see cref="MockCrmClient"/>;
    /// når den rigtige CRM-integration er klar, ændres kun denne ene linje.
    /// </summary>
    public static IServiceCollection AddCrmClient(this IServiceCollection services)
    {
        services.AddSingleton<ICrmClient, MockCrmClient>();
        return services;
    }
}
