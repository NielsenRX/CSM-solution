namespace MyCMSSolution.SelfService.Produktkort;

/// <summary>
/// Data til Produktkort-blokken. Se <see cref="PlaceholderProduktkortDataProvider"/> for hvorfor
/// dette bevidst ikke kommer fra <c>ICrmClient</c>/CRM-kontrakten endnu.
/// </summary>
public sealed record ProduktkortData(
    string ProduktNavn,
    string Hastighed,
    decimal NuvaerendePris,
    decimal NormalprisEfterKampagne,
    int KampagnePeriodeMaaneder,
    string SparBadgeTekst,
    string OprettelsesgebyrTekst,
    IReadOnlyList<string> Fordele,
    string MindsteprisOplysning);
