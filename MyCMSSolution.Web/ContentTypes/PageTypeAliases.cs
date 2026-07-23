namespace MyCMSSolution.Web.ContentTypes;

/// <summary>
/// Aliaser for de otte bekræftede sidetyper (kickoff-planens Spor 2) og deres fælles Block Grid-felt.
/// </summary>
public static class PageTypeAliases
{
    public const string Produktside = "produktside";
    public const string KampagneTilbud = "kampagneTilbud";
    public const string NyhedPresse = "nyhedPresse";
    public const string Driftsinfo = "driftsinfo";
    public const string Faq = "faq";
    public const string Jobopslag = "jobopslag";
    public const string OmKontaktVilkaar = "omKontaktVilkaar";
    public const string Partnerside = "partnerside";

    /// <summary>Fælles Block Grid-property til indholdsområdet på alle sidetyper.</summary>
    public const string Indholdsomraade = "indholdsomraade";

    /// <summary>
    /// Template-alias (= Views/{alias}.cshtml-filnavn) for hver sidetype (TASK-14). Holdt adskilt
    /// fra sidetypens dokumenttype-alias og bevidst PascalCase/ASCII-sikker, så den kan bruges direkte
    /// som filnavn - uafhængig af visningsnavne som "Kampagne/tilbud", der indeholder "/".
    /// </summary>
    public const string ProduktsideTemplateAlias = "Produktside";
    public const string KampagneTilbudTemplateAlias = "KampagneTilbud";
    public const string NyhedPresseTemplateAlias = "NyhedPresse";
    public const string DriftsinfoTemplateAlias = "Driftsinfo";
    public const string FaqTemplateAlias = "Faq";
    public const string JobopslagTemplateAlias = "Jobopslag";
    public const string OmKontaktVilkaarTemplateAlias = "OmKontaktVilkaar";
    public const string PartnersideTemplateAlias = "Partnerside";
}
