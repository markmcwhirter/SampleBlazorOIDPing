using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleBlazorOIDPing.Components;
using SampleBlazorOIDPing.Components.Account;
using SampleBlazorOIDPing.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var authBuilder = builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    });

authBuilder.AddIdentityCookies();

authBuilder.AddOpenIdConnect("oidc", options =>
    {
        var oidcConfig = builder.Configuration.GetSection("OpenIdConnect");
        
        options.Authority = oidcConfig["Authority"] ?? throw new InvalidOperationException("OpenIdConnect:Authority is required");
        options.ClientId = oidcConfig["ClientId"] ?? throw new InvalidOperationException("OpenIdConnect:ClientId is required");
        options.ClientSecret = oidcConfig["ClientSecret"] ?? throw new InvalidOperationException("OpenIdConnect:ClientSecret is required");
        options.ResponseType = oidcConfig["ResponseType"] ?? "code";
        options.SaveTokens = bool.Parse(oidcConfig["SaveTokens"] ?? "true");
        options.GetClaimsFromUserInfoEndpoint = bool.Parse(oidcConfig["GetClaimsFromUserInfoEndpoint"] ?? "true");
        options.CallbackPath = "/signin-oidc";
        
        // Add scopes
        var scopes = oidcConfig["Scope"]?.Split(' ') ?? new[] { "openid", "profile", "email" };
        options.Scope.Clear();
        foreach (var scope in scopes)
        {
            options.Scope.Add(scope);
        }
        
        // Map claims
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        
        // Handle events
        options.Events.OnRemoteFailure = context =>
        {
            context.HandleResponse();
            var errorMessage = context.Failure?.Message ?? "Authentication failed";
            context.Response.Redirect("/Account/Login?error=" + Uri.EscapeDataString(errorMessage));
            return Task.CompletedTask;
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
