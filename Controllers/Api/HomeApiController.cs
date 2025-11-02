using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GradeSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeApiController : ControllerBase
    {
        private readonly ILogger<HomeApiController> _logger;
        public HomeApiController(ILogger<HomeApiController> logger) => _logger = logger;

        public record HealthDto(string Status, DateTime Utc, string? TraceId);
        public record PrivacyDto(string Message);
        public record ErrorDto(string RequestId);

        [HttpGet]
        public IActionResult Get() =>
            Ok(new HealthDto("ok", DateTime.UtcNow, Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        [HttpGet("privacy")]
        public IActionResult Privacy() =>
            Ok(new PrivacyDto("This API does not collect personal data beyond logs required for diagnostics."));

        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            Problem(detail: "An error occurred.", instance: HttpContext.TraceIdentifier);
    }
}
