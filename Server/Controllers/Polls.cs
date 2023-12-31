using Microsoft.AspNetCore.Mvc;

namespace Letterboxd_Poll_Website.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PollsController : ControllerBase {
    private readonly ILogger<pollsController> _logger;

    public pollsController(ILogger<PollsController> logger) {
        _logger = logger;
    }

    [HttpPost(Name = "Create Poll")]
    public IEnumerable<WeatherForecast> Post() {
        Console.WriteLine("test");
    }
}