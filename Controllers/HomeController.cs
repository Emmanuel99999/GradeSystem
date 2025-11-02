using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GradeSystem.Models;

namespace GradeSystem.Controllers;

public class HomeApiController : Controller
{
    private readonly ILogger<HomeApiController> _logger;

    public HomeApiController(ILogger<HomeApiController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new User { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
