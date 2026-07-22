using MyCMSSolution.Core.Crm;

namespace MyCMSSolution.Crm.Client;

/// <summary>
/// Isolerer alle CRM-kald bag et interface, så en rigtig integration senere kan
/// erstatte <see cref="MockCrmClient"/> uden at ændre forbrugere som SelfService eller Web.
/// </summary>
public interface ICrmClient
{
    Task<CrmCustomer?> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default);
}
