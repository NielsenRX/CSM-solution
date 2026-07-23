using MyCMSSolution.SelfService.Produktkort;

namespace MyCMSSolution.Tests;

public class PlaceholderProduktkortDataProviderTests
{
    private readonly PlaceholderProduktkortDataProvider _sut = new();

    [Fact]
    public void GetByReference_ReturnsBredband500_ForKnownReference()
    {
        var data = _sut.GetByReference("bredband-500");

        Assert.Equal("Bredbånd 500/500", data.ProduktNavn);
        Assert.Equal("500/500 Mbit/s", data.Hastighed);
    }

    [Fact]
    public void GetByReference_ReturnsBredband1000_ForKnownReference()
    {
        var data = _sut.GetByReference("bredband-1000");

        Assert.Equal("Bredbånd 1000/1000", data.ProduktNavn);
        Assert.Equal("1000/1000 Mbit/s", data.Hastighed);
    }

    [Fact]
    public void GetByReference_ReturnsDifferentData_ForDifferentKnownReferences()
    {
        var data500 = _sut.GetByReference("bredband-500");
        var data1000 = _sut.GetByReference("bredband-1000");

        Assert.NotEqual(data500.ProduktNavn, data1000.ProduktNavn);
        Assert.NotEqual(data500.NuvaerendePris, data1000.NuvaerendePris);
    }

    [Fact]
    public void GetByReference_IsCaseInsensitive()
    {
        var data = _sut.GetByReference("BREDBAND-500");

        Assert.Equal("Bredbånd 500/500", data.ProduktNavn);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ukendt-produkt")]
    public void GetByReference_ReturnsFallback_ForMissingOrUnknownReference(string? produktreference)
    {
        var data = _sut.GetByReference(produktreference);

        Assert.Equal("Bredbånd 300/300", data.ProduktNavn);
    }

    [Theory]
    [InlineData("bredband-500")]
    [InlineData("bredband-1000")]
    [InlineData(null)]
    public void GetByReference_AlwaysIncludesLegallyRequiredMinimumPriceDisclosure(string? produktreference)
    {
        var data = _sut.GetByReference(produktreference);

        Assert.False(string.IsNullOrWhiteSpace(data.MindsteprisOplysning));
    }

    [Theory]
    [InlineData("bredband-500")]
    [InlineData("bredband-1000")]
    [InlineData(null)]
    public void GetByReference_AlwaysReturnsAtLeastOneFordel(string? produktreference)
    {
        var data = _sut.GetByReference(produktreference);

        Assert.NotEmpty(data.Fordele);
    }
}
