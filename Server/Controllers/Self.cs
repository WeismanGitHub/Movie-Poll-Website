﻿using Microsoft.AspNetCore.Mvc;
using API;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/self")]
public class SelfController : ControllerBase {
	private readonly Settings _settings;

	public SelfController(Settings settings) {
		_settings = settings;
	}

	[HttpGet("servers", Name = "GetGuilds")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetGuilds(string authCode) {
		Console.WriteLine(authCode);

		return Ok();
	}
}
