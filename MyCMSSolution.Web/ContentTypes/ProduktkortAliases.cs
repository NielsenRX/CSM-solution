namespace MyCMSSolution.Web.ContentTypes;

/// <summary>
/// Aliaser og en fast, kendt nøgle for Produktkort Block Grid-elementtypen (TASK-11).
/// Den faste nøgle gør det muligt at referere til elementtypen fra Block Grid-konfigurationen
/// uden at skulle slå den op efter oprettelse.
/// </summary>
public static class ProduktkortAliases
{
    public const string ElementTypeAlias = "produktkort";

    public static readonly Guid ElementTypeKey = Guid.Parse("6f1a2b3c-4d5e-4a6b-8c7d-9e0f1a2b3c4d");

    public const string ProduktReferencePropertyAlias = "produktreference";
}
