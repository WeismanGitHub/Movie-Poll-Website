using Microsoft.AspNetCore.Mvc;
using server;
using API;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/discord")]
public class DiscordController : ControllerBase {
	private readonly Settings _settings;

	public DiscordController(Settings settings) {
		_settings = settings;
	}

	[HttpGet("token", Name = "GetAccessToken")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAccessToken([FromQuery(Name = "code")] string authCode) {
		var discord = new DiscordOauth2(_settings);
		var accessToken = await discord.GetAccessToken(authCode);

		return Ok(accessToken);
	}

	[HttpGet("guilds", Name = "GetGuilds")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetGuilds([FromQuery(Name = "token")] string accessToken) {
		var discord = new DiscordOauth2(_settings);
		var guilds = await discord.GetGuilds(accessToken);

		return Ok(guilds);
	}
}
