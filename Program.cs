using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options => {
        options.Conventions.AuthorizePage("/Privacy");
    });

builder.Services.AddAuthentication(options => {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("cookie", options => {
        options.Cookie.Name = builder.Configuration["IdentityProvider:CookieName"];

        options.Events.OnSigningOut = async e => {
            await e.HttpContext.RevokeUserRefreshTokenAsync();
        };
    })
    .AddOpenIdConnect("oidc", options => {
        options.Authority = builder.Configuration["IdentityProvider:Authority"];
        
        options.ClientId = builder.Configuration["IdentityProvider:ClientId"];
        options.ClientSecret = builder.Configuration["IdentityProvider:ClientSecret"];
        
        options.Scope.Add("openid");

        options.ResponseType = "code";
        //options.RequireHttpsMetadata = false;
        
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["IdentityProvider:ClientId"]
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