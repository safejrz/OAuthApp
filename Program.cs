using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");

var app = builder.Build();


app.MapGet("/login", () => Results.SignIn(
    new ClaimsPrincipal(
        new ClaimsIdentity(
            new[] { new Claim("user_id", Guid.NewGuid().ToString()) },
            "cookie"
        )
    ),
    authenticationScheme: "cookie"
    ));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
