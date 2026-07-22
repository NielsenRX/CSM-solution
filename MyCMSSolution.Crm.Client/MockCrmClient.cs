using MyCMSSolution.Core.Crm;

namespace MyCMSSolution.Crm.Client;

/// <summary>
/// Midlertidig in-memory implementering af <see cref="ICrmClient"/>, indtil det rigtige
/// CRM-system er valgt og integreret. Udskiftes ved at registrere en ny implementering
/// i <see cref="ServiceCollectionExtensions.AddCrmClient"/>.
/// </summary>
public sealed class MockCrmClient : ICrmClient
{
    private static readonly IReadOnlyDictionary<string, CrmCustomer> Customers = new Dictionary<string, CrmCustomer>
    {
        ["1"] = new CrmCustomer("1", "Test Testesen", "test@example.com"),
    };

    public Task<CrmCustomer?> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        Customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }
}
