using MyCMSSolution.Core.Crm;
using MyCMSSolution.Crm.Client;

namespace MyCMSSolution.Tests;

public class MockCrmClientTests
{
    private readonly MockCrmClient _sut = new();

    [Fact]
    public async Task GetCustomerProfile_ReturnsProfile_ForKnownCustomer()
    {
        var profile = await _sut.GetCustomerProfile("1001");

        Assert.NotNull(profile);
        Assert.Equal("1001", profile!.CustomerId);
        Assert.Equal("Anna Andersen", profile.Name);
        Assert.Equal(CrmCustomerType.B2C, profile.CustomerType);
    }

    [Fact]
    public async Task GetCustomerProfile_ReturnsNull_ForUnknownCustomer()
    {
        var profile = await _sut.GetCustomerProfile("does-not-exist");

        Assert.Null(profile);
    }

    [Fact]
    public async Task GetCustomerProfile_DistinguishesB2BFromB2C()
    {
        var profile = await _sut.GetCustomerProfile("1003");

        Assert.NotNull(profile);
        Assert.Equal(CrmCustomerType.B2B, profile!.CustomerType);
    }

    [Fact]
    public async Task GetSubscriptions_ReturnsSubscriptions_ForKnownCustomer()
    {
        var subscriptions = await _sut.GetSubscriptions("1001");

        var subscription = Assert.Single(subscriptions);
        Assert.Equal(CrmProductType.Broadband, subscription.ProductType);
        Assert.Equal(CrmSubscriptionStatus.Active, subscription.Status);
    }

    [Fact]
    public async Task GetSubscriptions_ReturnsEmptyList_ForUnknownCustomer()
    {
        var subscriptions = await _sut.GetSubscriptions("does-not-exist");

        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task GetSubscriptions_IncludesSubscriptionInBindingPeriod()
    {
        var subscriptions = await _sut.GetSubscriptions("1002");

        var boundSubscription = Assert.Single(subscriptions, s => s.SubscriptionId == "SUB-1002-1");
        Assert.NotNull(boundSubscription.BindingPeriodEndDate);
        Assert.True(boundSubscription.BindingPeriodEndDate > DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task GetSubscriptions_CanContainSubscriptionsWithoutBinding()
    {
        var subscriptions = await _sut.GetSubscriptions("1002");

        var unboundSubscription = Assert.Single(subscriptions, s => s.SubscriptionId == "SUB-1002-2");
        Assert.Null(unboundSubscription.BindingPeriodEndDate);
    }

    [Fact]
    public async Task GetInvoices_ReturnsInvoices_ForKnownCustomer()
    {
        var invoices = await _sut.GetInvoices("1001");

        Assert.Equal(2, invoices.Count);
        Assert.Contains(invoices, i => i.Status == CrmInvoiceStatus.Paid);
        Assert.Contains(invoices, i => i.Status == CrmInvoiceStatus.Unpaid);
    }

    [Fact]
    public async Task GetInvoices_ReturnsEmptyList_ForUnknownCustomer()
    {
        var invoices = await _sut.GetInvoices("does-not-exist");

        Assert.Empty(invoices);
    }

    [Fact]
    public async Task GetInvoices_CanContainOverdueInvoice()
    {
        var invoices = await _sut.GetInvoices("1002");

        Assert.Contains(invoices, i => i.Status == CrmInvoiceStatus.Overdue);
    }
}
