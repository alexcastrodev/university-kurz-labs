namespace Shipping.Tests;

[TestClass]
public sealed class PromoCodeTests
{
    [TestMethod]
    [DataRow("FREESHIP")]
    [DataRow("freeship", DisplayName = "the code is case-insensitive")]
    [DataRow("  FreeShip ", DisplayName = "surrounding spaces are ignored")]
    public void TheFreeShippingCodeMakesADomesticParcelFree(string code) =>
        Assert.AreEqual(0m, ShippingCalculator.Quote(500, Zone.Domestic, promoCode: code));

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("FREESHIP10")]
    [DataRow("FREE SHIP")]
    public void AnyOtherCodePaysTheBandPrice(string? code) =>
        Assert.AreEqual(5.00m, ShippingCalculator.Quote(500, Zone.Domestic, promoCode: code));

    [TestMethod]
    public void TheCodeDoesNotWorkAbroad() =>
        Assert.AreEqual(7.50m, ShippingCalculator.Quote(500, Zone.Neighbor, promoCode: "FREESHIP"));
}
