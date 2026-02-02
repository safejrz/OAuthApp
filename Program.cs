using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
var azureAdSection = builder.Configuration.GetSection("AzureAd");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        var tenantId = azureAdSection["TenantId"] ?? "common";
        options.Authority = $"{azureAdSection["Instance"]}{tenantId}/v2.0";
        options.ClientId = azureAdSection["ClientId"] ?? throw new InvalidOperationException("Missing AzureAd:ClientId configuration.");
        options.ClientSecret = azureAdSection["ClientSecret"] ?? throw new InvalidOperationException("Missing AzureAd:ClientSecret configuration.");
        options.CallbackPath = azureAdSection["CallbackPath"] ?? "/signin-oidc";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.Scope.Add("offline_access");
    });

var app = builder.Build();

app.MapGet("/login", () =>
    Results.Challenge(new AuthenticationProperties
    {
        RedirectUri = "/"
    }, new[] { OpenIdConnectDefaults.AuthenticationScheme }));

app.MapGet("/logout", () =>
    Results.SignOut(new AuthenticationProperties
    {
        RedirectUri = "/"
    }, new[]
    {
        CookieAuthenticationDefaults.AuthenticationScheme,
        OpenIdConnectDefaults.AuthenticationScheme
    }));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
