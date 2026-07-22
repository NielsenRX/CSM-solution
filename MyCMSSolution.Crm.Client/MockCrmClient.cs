using MyCMSSolution.Core.Crm;

namespace MyCMSSolution.Crm.Client;

/// <summary>
/// Midlertidig in-memory implementering af <see cref="ICrmClient"/>, indtil det rigtige
/// CRM-system er valgt og integreret. Udskiftes ved at registrere en ny implementering
/// i <see cref="ServiceCollectionExtensions.AddCrmClient"/>.
/// </summary>
public sealed class MockCrmClient : ICrmClient
{
    private static readonly IReadOnlyDictionary<string, CrmCustomerProfile> Profiles = new Dictionary<string, CrmCustomerProfile>
    {
        ["1001"] = new CrmCustomerProfile(
            CustomerId: "1001",
            Name: "Anna Andersen",
            Email: "anna.andersen@example.dk",
            Phone: "+45 20 12 34 56",
            CustomerType: CrmCustomerType.B2C,
            Address: "Storegade 12, 3700 Rønne"),

        ["1002"] = new CrmCustomerProfile(
            CustomerId: "1002",
            Name: "Peter Poulsen",
            Email: "peter.poulsen@example.dk",
            Phone: "+45 30 98 76 54",
            CustomerType: CrmCustomerType.B2C,
            Address: "Skovvej 4, 4000 Roskilde"),

        ["1003"] = new CrmCustomerProfile(
            CustomerId: "1003",
            Name: "Nielsen & Søn ApS",
            Email: "kontakt@nielsenogsoen.dk",
            Phone: "+45 70 11 22 33",
            CustomerType: CrmCustomerType.B2B,
            Address: "Erhvervsparken 9, 8000 Aarhus C"),
    };

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<CrmSubscription>> Subscriptions = new Dictionary<string, IReadOnlyList<CrmSubscription>>
    {
        ["1001"] =
        [
            new CrmSubscription(
                SubscriptionId: "SUB-1001-1",
                ProductName: "Bredbånd 1000/1000",
                ProductType: CrmProductType.Broadband,
                Speed: "1000/1000 Mbit/s",
                Price: 499m,
                StartDate: new DateOnly(2023, 3, 1),
                BindingPeriodEndDate: null,
                Status: CrmSubscriptionStatus.Active,
                AddOns: ["Ekstra WiFi-udstyr"]),
        ],

        // Kunde med et abonnement i binding (bindingPeriodEndDate sat) — til test af binding-scenariet.
        ["1002"] =
        [
            new CrmSubscription(
                SubscriptionId: "SUB-1002-1",
                ProductName: "Bredbånd 600/600",
                ProductType: CrmProductType.Broadband,
                Speed: "600/600 Mbit/s",
                Price: 349m,
                StartDate: new DateOnly(2025, 1, 15),
                BindingPeriodEndDate: new DateOnly(2027, 1, 15),
                Status: CrmSubscriptionStatus.Active,
                AddOns: []),
            new CrmSubscription(
                SubscriptionId: "SUB-1002-2",
                ProductName: "TV Basispakke",
                ProductType: CrmProductType.Tv,
                Speed: null,
                Price: 199m,
                StartDate: new DateOnly(2025, 1, 15),
                BindingPeriodEndDate: null,
                Status: CrmSubscriptionStatus.Active,
                AddOns: ["Ekstra TV-boks"]),
        ],

        ["1003"] =
        [
            new CrmSubscription(
                SubscriptionId: "SUB-1003-1",
                ProductName: "Erhverv Bredbånd 500/500",
                ProductType: CrmProductType.Broadband,
                Speed: "500/500 Mbit/s",
                Price: 899m,
                StartDate: new DateOnly(2024, 9, 1),
                BindingPeriodEndDate: null,
                Status: CrmSubscriptionStatus.Active,
                AddOns: ["Statisk IP"]),
            new CrmSubscription(
                SubscriptionId: "SUB-1003-2",
                ProductName: "Streaming Erhverv",
                ProductType: CrmProductType.Streaming,
                Speed: null,
                Price: 149m,
                StartDate: new DateOnly(2026, 7, 1),
                BindingPeriodEndDate: null,
                Status: CrmSubscriptionStatus.Pending,
                AddOns: []),
        ],
    };

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<CrmInvoice>> Invoices = new Dictionary<string, IReadOnlyList<CrmInvoice>>
    {
        ["1001"] =
        [
            new CrmInvoice(
                InvoiceId: "INV-1001-06",
                InvoiceDate: new DateOnly(2026, 6, 1),
                DueDate: new DateOnly(2026, 6, 15),
                Amount: 499m,
                Status: CrmInvoiceStatus.Paid,
                DownloadUrl: "https://faktura.example.dk/1001/2026-06.pdf"),
            new CrmInvoice(
                InvoiceId: "INV-1001-07",
                InvoiceDate: new DateOnly(2026, 7, 1),
                DueDate: new DateOnly(2026, 7, 15),
                Amount: 499m,
                Status: CrmInvoiceStatus.Unpaid,
                DownloadUrl: "https://faktura.example.dk/1001/2026-07.pdf"),
        ],

        ["1002"] =
        [
            new CrmInvoice(
                InvoiceId: "INV-1002-05",
                InvoiceDate: new DateOnly(2026, 5, 1),
                DueDate: new DateOnly(2026, 5, 15),
                Amount: 548m,
                Status: CrmInvoiceStatus.Overdue,
                DownloadUrl: "https://faktura.example.dk/1002/2026-05.pdf"),
            new CrmInvoice(
                InvoiceId: "INV-1002-06",
                InvoiceDate: new DateOnly(2026, 6, 1),
                DueDate: new DateOnly(2026, 6, 15),
                Amount: 548m,
                Status: CrmInvoiceStatus.Paid,
                DownloadUrl: "https://faktura.example.dk/1002/2026-06.pdf"),
        ],

        ["1003"] =
        [
            new CrmInvoice(
                InvoiceId: "INV-1003-06",
                InvoiceDate: new DateOnly(2026, 6, 1),
                DueDate: new DateOnly(2026, 6, 15),
                Amount: 899m,
                Status: CrmInvoiceStatus.Paid,
                DownloadUrl: "https://faktura.example.dk/1003/2026-06.pdf"),
            new CrmInvoice(
                InvoiceId: "INV-1003-07",
                InvoiceDate: new DateOnly(2026, 7, 1),
                DueDate: new DateOnly(2026, 7, 15),
                Amount: 899m,
                Status: CrmInvoiceStatus.Unpaid,
                DownloadUrl: "https://faktura.example.dk/1003/2026-07.pdf"),
        ],
    };

    public Task<CrmCustomerProfile?> GetCustomerProfile(string customerId, CancellationToken cancellationToken = default)
    {
        Profiles.TryGetValue(customerId, out var profile);
        return Task.FromResult(profile);
    }

    public Task<IReadOnlyList<CrmSubscription>> GetSubscriptions(string customerId, CancellationToken cancellationToken = default)
    {
        var subscriptions = Subscriptions.TryGetValue(customerId, out var found)
            ? found
            : [];
        return Task.FromResult(subscriptions);
    }

    public Task<IReadOnlyList<CrmInvoice>> GetInvoices(string customerId, CancellationToken cancellationToken = default)
    {
        var invoices = Invoices.TryGetValue(customerId, out var found)
            ? found
            : [];
        return Task.FromResult(invoices);
    }
}
