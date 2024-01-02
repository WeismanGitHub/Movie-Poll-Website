using Microsoft.AspNetCore.Mvc;

namespace Letterboxd_Poll_Website.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PollsController : ControllerBase {

    [HttpPost(Name = "Create Poll")]
    public string Post() {
        Console.WriteLine("test");
		return "test";
    }
}
