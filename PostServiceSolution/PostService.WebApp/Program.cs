using System.Globalization;
using PostService.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<PostingApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PostServiceApi:BaseUrl"] ?? "http://localhost:5036/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
var app = builder.Build();
app.UseExceptionHandler("/Error");
var culture = CultureInfo.GetCultureInfo("uk-UA");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture),
    SupportedCultures = [culture], SupportedUICultures = [culture]
});
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
