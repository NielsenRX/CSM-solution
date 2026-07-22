namespace MyCMSSolution.Core.Crm;

/// <summary>status fra CRM-API-kontrakten (subscriptions-endpointet): aktiv/opsagt/afventer.</summary>
public enum CrmSubscriptionStatus
{
    Active,
    Cancelled,
    Pending,
}
