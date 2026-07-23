namespace MyCMSSolution.SelfService.Produktkort;

/// <summary>
/// MIDLERTIDIG PLACEHOLDER-DATAKILDE (TASK-11).
///
/// Denne klasse er BEVIDST ikke koblet til <c>ICrmClient</c>/CRM-kontrakten - den kontrakt
/// ændres/udvides ikke nu (jf. kickoff-planens Åbne spørgsmål #3). Data herunder er hardkodede
/// eksempler til at bygge og teste selve Produktkort-blokken og dens visning.
///
/// Den reelle datakilde bliver sandsynligvis dækningstjek-endpointet (GET /coverage?address=),
/// som allerede returnerer "availableProducts" - denne klasse er stedet, hvor det kobles på,
/// når det bliver nødvendigt (samme offentlige interface, <see cref="IProduktkortDataProvider"/>,
/// så resten af blokken ikke skal ændres).
/// </summary>
public sealed class PlaceholderProduktkortDataProvider : IProduktkortDataProvider
{
    private static readonly ProduktkortData Standardprodukt = new(
        ProduktNavn: "Bredbånd 300/300",
        Hastighed: "300/300 Mbit/s",
        NuvaerendePris: 249m,
        NormalprisEfterKampagne: 349m,
        KampagnePeriodeMaaneder: 6,
        SparBadgeTekst: "Spar 100 kr/md i 6 måneder",
        OprettelsesgebyrTekst: "Gratis oprettelse i kampagneperioden",
        Fordele: new[]
        {
            "Fri installation",
            "Ingen binding",
            "Gratis router inkluderet",
        },
        MindsteprisOplysning: "Mindstepris ved 6 måneders kampagneperiode: 1.494 kr (249 kr x 6 måneder).");

    private static readonly IReadOnlyDictionary<string, ProduktkortData> Eksempelprodukter =
        new Dictionary<string, ProduktkortData>(StringComparer.OrdinalIgnoreCase)
        {
            ["bredband-500"] = new ProduktkortData(
                ProduktNavn: "Bredbånd 500/500",
                Hastighed: "500/500 Mbit/s",
                NuvaerendePris: 299m,
                NormalprisEfterKampagne: 399m,
                KampagnePeriodeMaaneder: 6,
                SparBadgeTekst: "Spar 100 kr/md i 6 måneder",
                OprettelsesgebyrTekst: "Gratis oprettelse i kampagneperioden",
                Fordele: new[]
                {
                    "Fri installation",
                    "Ingen binding",
                    "Gratis router inkluderet",
                    "Fri fart op og ned",
                },
                MindsteprisOplysning: "Mindstepris ved 6 måneders kampagneperiode: 1.794 kr (299 kr x 6 måneder)."),

            ["bredband-1000"] = new ProduktkortData(
                ProduktNavn: "Bredbånd 1000/1000",
                Hastighed: "1000/1000 Mbit/s",
                NuvaerendePris: 399m,
                NormalprisEfterKampagne: 499m,
                KampagnePeriodeMaaneder: 6,
                SparBadgeTekst: "Spar 100 kr/md i 6 måneder",
                OprettelsesgebyrTekst: "Oprettelse: 499 kr",
                Fordele: new[]
                {
                    "Fri installation",
                    "Ingen binding",
                    "Gratis router inkluderet",
                    "Prioriteret support",
                },
                MindsteprisOplysning: "Mindstepris ved 6 måneders kampagneperiode: 2.893 kr (399 kr x 6 måneder + 499 kr i oprettelse)."),
        };

    public ProduktkortData GetByReference(string? produktreference)
    {
        if (!string.IsNullOrWhiteSpace(produktreference)
            && Eksempelprodukter.TryGetValue(produktreference, out ProduktkortData data))
        {
            return data;
        }

        return Standardprodukt;
    }
}
