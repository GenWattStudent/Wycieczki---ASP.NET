using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Book.App.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Book.App.ViewModels;

namespace Book.App.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ClaimsData claimsData = new ClaimsData
        {
            Name = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Role = User.FindFirst(ClaimTypes.Role) != null ? (Role)Enum.Parse(typeof(Role), User.FindFirst(ClaimTypes.Role)?.Value) : Role.User
        };

        return View(claimsData);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
