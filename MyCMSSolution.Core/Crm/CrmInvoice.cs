namespace MyCMSSolution.Core.Crm;

/// <summary>
/// Svarer til felterne i GET /customer/{id}/invoices fra CRM-API-kontrakten
/// (docs/kickoff-plan-fase3-cms-mycmssolution.md, "Spor 3").
/// </summary>
public sealed record CrmInvoice(
    string InvoiceId,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    decimal Amount,
    CrmInvoiceStatus Status,
    string DownloadUrl);
