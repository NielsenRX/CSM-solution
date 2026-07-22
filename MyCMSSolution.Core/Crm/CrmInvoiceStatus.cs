namespace MyCMSSolution.Core.Crm;

/// <summary>status fra CRM-API-kontrakten (invoices-endpointet): betalt/ikke betalt/forfalden.</summary>
public enum CrmInvoiceStatus
{
    Paid,
    Unpaid,
    Overdue,
}
