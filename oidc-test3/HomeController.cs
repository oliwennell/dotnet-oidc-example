using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace oidc_test3;

public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return Ok("hello from index, this is completely unauthenticated");
    }
    
    [Authorize] // If not already authenticated this kicks off the process
    public IActionResult Protected()
    {
        return Ok("hello from protected, only authenticated identies should see this");
    }
    
    [Authorize(Policy = "IsAdminPolicy")]
    public IActionResult AdminArea()
    {
        return Ok("hello from secret admin area, only admins should be able to see this");
    }

    public async Task Logout()
    {
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return Ok("oopsie!");
    }
}