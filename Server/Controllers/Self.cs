using Microsoft.AspNetCore.Mvc;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/self")]
public class SelfController : ControllerBase {
	[HttpGet(Name = "GetGuilds")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetGuilds() {
		var authCode = Request.Headers.Authorization;
		Console.WriteLine(authCode);

		return Ok();
	}
}
