using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace oidc_test3;

public class ClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Dummy logic to work out if the principal is an admin or not.
        // Relies on the standard "name" claim that the identity provider should have already set.
        var shouldBeAdmin = principal.HasClaim(ClaimTypes.Name, "Oli Wennell");

        if (shouldBeAdmin)
        {
            // Links up with what we set in builder.Services.AddAuthorization in Program.cs
            principal.Identities.First().AddClaim(new Claim("IsAdminClaim", "true"));
        }
        
        return Task.FromResult(principal);
    }
}