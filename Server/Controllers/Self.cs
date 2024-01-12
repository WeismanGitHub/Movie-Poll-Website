using Microsoft.AspNetCore.Mvc;
using server;
using API;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/self")]
public class SelfController : ControllerBase {
	private readonly Settings _settings;

	public SelfController(Settings settings) {
		_settings = settings;
	}

	[HttpGet("guilds", Name = "GetGuilds")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetGuilds([FromQuery(Name = "code")] string authCode) {
		var utils = new Utils(_settings);
		var guilds = await utils.GetGuilds(authCode);

		return Ok(guilds);
	}
}
