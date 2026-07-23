namespace MyCMSSolution.SelfService.Produktkort;

/// <summary>
/// Henter data til en Produktkort-blok ud fra den produktreference, marketing har sat i CMS'et.
/// </summary>
public interface IProduktkortDataProvider
{
    /// <summary>
    /// Returnerer data for det angivne produkt. Returnerer altid et fallback-produkt for
    /// ukendt, tom eller manglende reference - blokken må aldrig crashe pga. CMS-opsætning.
    /// </summary>
    ProduktkortData GetByReference(string? produktreference);
}
