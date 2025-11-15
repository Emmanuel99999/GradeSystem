using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(); // Buscará Views/Home/Index.cshtml
        }

        public IActionResult Privacy()
        {
            return View(); // Views/Home/Privacy.cshtml si existe
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
