using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Database;
using server;
using API;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsController : ControllerBase {
	private readonly Settings _settings;

	public PollsController(Settings settings) {
		_settings = settings;
	}

	public class CreatePollDTO {
		[Required, MaxLength(500), MinLength(1)]
		public required string Question { get; set; }
		public DateTime? Expiration { get; set; }
		public string? GuildId { get; set; }
		public string? AuthCode { get; set; }
		[Required, MinLength(2), MaxLength(50)]
		public required List<Movie> Movies {  get; set; }
	}

	[HttpPost(Name = "CreatePoll")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreatePoll(CreatePollDTO input) {
		using var db = new LbPollContext();
		var client = new HttpClient();

		if (input.Expiration != null && input.Expiration <= DateTime.Now) {
			return BadRequest("Expiration must be greater than the current time.");
		} else if (input.Movies.Count < 2 || input.Movies.Count > 50) {
			return BadRequest("Selected movies must be between 2 and 50.");
		}

		if (input.Movies.Any(Movie => Movie.Id.Length > 50)) {
			return BadRequest("Invalid Ids.");
		}

		if (input.GuildId != null) {
			if (input.AuthCode == null) {
				return BadRequest("You must be logged in to restrict a poll to a server.");
			}

			var utils = new Utils(_settings);
			var accessToken = await utils.GetAccessToken(input.AuthCode);
			var guilds = await utils.GetGuilds(accessToken);

			if (!guilds.Any(guild => guild.id == input.GuildId)) {
				return BadRequest("You must be in the server to restrict this poll.");
			}
		}

		var poll = new Poll {
			Question = input.Question,
			Expiration = input.Expiration,
			GuildId = input.GuildId,
			Movies = input.Movies,
		};

		db.Add(poll);
		await db.SaveChangesAsync();

		return Created($"/polls/{poll.Id}", poll.Id);
    }

	public class PollResponse {
		public required string Question { get; set; }
		public required IEnumerable<string> Votes {  get; set; }
		public required List<Movie> Movies {  get; set; }
		public required DateTime CreatedAt { get; set; }
		public required bool ServerRestricted { get; set; }
		public DateTime? Expiration { get; set; }
	}

	[HttpGet(Name = "GetPoll")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetPoll(Guid id) {
		using var db = new LbPollContext();
		var client = new HttpClient();

		var poll = await db.FindAsync<Poll>(id);

		if (poll == null) {
			return NotFound("Could not find poll.");
		}
		
		return Ok(new PollResponse() {
			Question = poll.Question,
			Votes = poll.Votes.Select(vote => vote.MovieId),
			Expiration = poll.Expiration,
			Movies = poll.Movies,
			ServerRestricted = poll.GuildId != null,
			CreatedAt = poll.CreatedAt,
		});
	}

	public class VoteDTO {
		public required string AuthCode { get; set; }
		public required string MovieId { get; set; }
	}

	[HttpPost("vote/{id}", Name = "Vote")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Vote(Guid id, VoteDTO vote) {
		using var db = new LbPollContext();
		var utils = new Utils(_settings);
		var client = new HttpClient();

		if (vote.AuthCode == null) {
			return BadRequest("Missing auth code.");
		}

		var accessToken = await utils.GetAccessToken(vote.AuthCode);
		var user = await utils.GetUser(accessToken);

		if (user == null) {
			return Unauthorized("Could not get your account.");
		}

		var poll = await db.FindAsync<Poll>(id);

		if (poll == null) {
			return NotFound("Could not find poll.");
		}

		if (!poll.Movies.Any((Movie) => Movie.Id == vote.MovieId)) {
			return BadRequest("This movie isn't an option.");
		}

		if (poll.Expiration != null && poll.Expiration < DateTime.Now) {
			return Forbid("Poll has expired.");
		}

		if (poll.GuildId != null) {
			var guilds = await utils.GetGuilds(accessToken);

			if (!guilds.Any(guild => guild.id == poll.GuildId)) {
				return BadRequest("This poll is restricted to a server that you aren't in.");
			}
		}

		poll.Votes.Add(new() {
			MovieId = vote.MovieId,
			UserId = user.Id
		});

		return Ok();
	}
}
