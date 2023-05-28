using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        // To secure all pages.
        // options.Conventions.AuthorizeFolder("/");

        // To only secure a specific page.
        options.Conventions.AuthorizePage("/Privacy");

        // To allow access without log in to a specific page.
        // options.Conventions.AllowAnonymousToPage("/Privacy");
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //options.Cookie.SameSite = SameSiteMode.Strict;

    options.Cookie.Name = builder.Configuration["IdentityProvider:CookieName"];
    options.Events.OnSigningOut = async e => await e.HttpContext.RevokeUserRefreshTokenAsync();
})
.AddOpenIdConnect(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.Authority = builder.Configuration["IdentityProvider:Authority"];
    options.ClientId = builder.Configuration["IdentityProvider:ClientId"];
    options.ClientSecret = builder.Configuration["IdentityProvider:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.ResponseMode = OpenIdConnectResponseMode.Query;
    options.RequireHttpsMetadata = true;
    //options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("openid");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = options.Authority,
        ValidAudience = options.ClientId,
        ValidateIssuer = true,
        ValidateAudience = true,
    };

    options.SaveTokens = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();