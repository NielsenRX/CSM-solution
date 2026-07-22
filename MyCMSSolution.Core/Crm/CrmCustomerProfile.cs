namespace MyCMSSolution.Core.Crm;

/// <summary>
/// Svarer til felterne i GET /customer/{id}/profile fra CRM-API-kontrakten
/// (docs/kickoff-plan-fase3-cms-mycmssolution.md, "Spor 3").
/// </summary>
public sealed record CrmCustomerProfile(
    string CustomerId,
    string Name,
    string Email,
    string Phone,
    CrmCustomerType CustomerType,
    string Address);
