
using MyCMSSolution.Crm.Client;
using MyCMSSolution.SelfService.Produktkort;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

AppDomain.CurrentDomain.SetData("DataDirectory",
    Path.Combine(builder.Environment.ContentRootPath, "umbraco", "Data"));

builder.Services.AddCrmClient();
builder.Services.AddProduktkortDataProvider();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();


await app.BootUmbracoAsync();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
