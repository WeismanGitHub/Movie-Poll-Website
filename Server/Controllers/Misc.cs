using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using server;
using API;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api")]
public class MiscController : ControllerBase {
	private readonly Settings _settings;

	public MiscController(Settings settings) {
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

	[HttpGet("search", Name = "SearchMovie")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> SearchMovie([FromQuery(Name = "query")] string query, [FromQuery(Name = "page")] string page) {
		if (query.Length > 500 || query.Length < 1) {
			return BadRequest("Query must be between 1 and 500 characters.");
		} else if (page.Length > 500 || page.Length < 1) {
			return BadRequest("Page must be between 1 and 500.");
		}

		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.TmdbKey);

		var res = await client.GetAsync($"https://api.themoviedb.org/3/search/movie?query={query}&include_adult=true&page={page}'");

		if (res.StatusCode == HttpStatusCode.BadRequest) {
			return BadRequest("Invalid search request.");
		} else if (!res.IsSuccessStatusCode) {
			throw new Exception("Search failed.");
		}

		var content = await res.Content.ReadAsStringAsync();

		return Ok(content);
	}
}
