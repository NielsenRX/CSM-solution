using Microsoft.AspNetCore.Mvc;
using MyCMSSolution.SelfService.Produktkort;

namespace MyCMSSolution.Web.ViewComponents;

/// <summary>
/// "Controlleren" i mønstret CMS-styret ramme + data bag en controller (jf. kickoff-planens
/// abonnementsoversigt-skabelon). Adskiller den CMS-redigerbare produktreference fra selve
/// produktdataene, som hentes via <see cref="IProduktkortDataProvider"/>.
/// </summary>
public class ProduktkortViewComponent : ViewComponent
{
    private readonly IProduktkortDataProvider _dataProvider;

    public ProduktkortViewComponent(IProduktkortDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public IViewComponentResult Invoke(string? produktreference)
    {
        ProduktkortData data = _dataProvider.GetByReference(produktreference);
        return View(data);
    }
}
