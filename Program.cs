using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Okta.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services
    .ConfigureApplicationCookie(options =>
    {
        // true means the cookie will only be passed to HTTP requests
        // and is NOT available to JavaScript on the page.
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOktaMvc(new OktaMvcOptions
    {
        OktaDomain = configuration["Okta:OktaDomain"],
        AuthorizationServerId = configuration["Okta:AuthorizationServerId"],
        ClientId = configuration["Okta:ClientId"],
        ClientSecret = configuration["Okta:ClientSecret"],
        //CallbackPath =
        // PostLogoutRedirectUri
        Scope = new List<string> { "openid", "profile", "email" },
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
