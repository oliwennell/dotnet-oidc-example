using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using oidc_test3;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        // These values are created by the identity provider (I used console.cloud.google.com)
        // Then I set them as env vars so I didn't check them in to git accidentally.
        options.Authority = Environment.GetEnvironmentVariable("OIDC_AUTHORITY");
        options.ClientId = Environment.GetEnvironmentVariable("OIDC_CLIENTID");
        options.ClientSecret = Environment.GetEnvironmentVariable("OIDC_CLIENTSECRET");
        
        options.ResponseType = "code";

        // This gets automatically implemented for us by .NET. It must match what's configured in the identity provider.
        options.CallbackPath = new PathString("/callback");
        
        options.SignedOutCallbackPath = new PathString("/signout");
    });

builder.Services.AddAuthorization(options =>
{
    // Define a policy we can use to restrict access to authenticated requests from admin users.
    // The ClaimsTransformation class decides if an identity is an admin or not.
    // If an identity is an admin, it gives it the "IsAdminClaim" claim. 
    options.AddPolicy("IsAdminPolicy",
        policyBuilder => policyBuilder.RequireAuthenticatedUser().RequireClaim("IsAdminClaim", "true").Build());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();