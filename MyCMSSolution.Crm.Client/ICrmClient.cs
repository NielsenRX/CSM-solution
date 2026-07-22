using MyCMSSolution.Core.Crm;

namespace MyCMSSolution.Crm.Client;

/// <summary>
/// Isolerer alle CRM-kald bag et interface, så en rigtig integration senere kan
/// erstatte <see cref="MockCrmClient"/> uden at ændre forbrugere som SelfService eller Web.
/// </summary>
public interface ICrmClient
{
    Task<CrmCustomerProfile?> GetCustomerProfile(string customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CrmSubscription>> GetSubscriptions(string customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CrmInvoice>> GetInvoices(string customerId, CancellationToken cancellationToken = default);
}
