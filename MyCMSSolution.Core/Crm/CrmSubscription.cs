namespace MyCMSSolution.Core.Crm;

/// <summary>
/// Svarer til felterne i GET /customer/{id}/subscriptions fra CRM-API-kontrakten
/// (docs/kickoff-plan-fase3-cms-mycmssolution.md, "Spor 3").
/// </summary>
/// <param name="Speed">Hastighed — kun relevant for <see cref="CrmProductType.Broadband"/>.</param>
/// <param name="BindingPeriodEndDate">Dato hvor binding udløber — null hvis ingen binding.</param>
/// <param name="AddOns">Liste over tilvalg, fx ekstra WiFi-udstyr.</param>
public sealed record CrmSubscription(
    string SubscriptionId,
    string ProductName,
    CrmProductType ProductType,
    string? Speed,
    decimal Price,
    DateOnly StartDate,
    DateOnly? BindingPeriodEndDate,
    CrmSubscriptionStatus Status,
    IReadOnlyList<string> AddOns);
