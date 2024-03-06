using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Server.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsController : ControllerBase {



	public class Movie {
		public string Id { get; set; }
		public string PosterPath { get; set; }
		public string ReleaseDate { get; set; }
		public string Title { get; set; }
	}

	public class PollResponse {
		public required string Question { get; set; }
		public required IEnumerable<string> Votes {  get; set; }
		public required ICollection<Movie> Movies {  get; set; }
		public required DateTime CreatedAt { get; set; }
		public required bool ServerRestricted { get; set; }
		public DateTime? Expiration { get; set; }
	}

	private class PollWithoutVoters {
		public required Guid Id;
		public required string Question;
		public required List<string> Votes;
		public string? GuildId;
		public required DateTime CreatedAt;
		public DateTime? Expiration;
		public List<string> MovieIds;
	}

	private class MovieDetails {
		// there are more details but I don't need them
		public int id { get; set; }
		public string title { get; set; }
		public string poster_path {  get; set; }
		public string release_date { get; set; }
	}

	[HttpGet("{id}", Name = "GetPoll")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetPoll([FromRoute] Guid id) {
		using var db = new LbPollContext();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.TmdbKey);

		var poll = await db.Polls
			.Where(p => p.Id == id)
			.Select(p => new PollWithoutVoters {
				Question = p.Question,
				Expiration = p.Expiration,
				CreatedAt = p.CreatedAt,
				GuildId = p.GuildId,
				Id = p.Id,
				MovieIds = p.MovieIds,
				Votes = p.Votes.Select(vote => vote.MovieId).ToList(),
			}).FirstOrDefaultAsync();

		if (poll == null) {
			return NotFound("Poll not found");
		}

		var movies = await Task.WhenAll(poll.MovieIds.Select(async (id) => {
			var res = await client.GetAsync($"https://api.themoviedb.org/3/movie/{id}");

			if (!res.IsSuccessStatusCode) {
				return new Movie {
					Id = id,
					PosterPath = null,
					Title = null,
					ReleaseDate = null,
				};
			}

			var movieDetails = JsonSerializer.Deserialize<MovieDetails>(await res.Content.ReadAsStringAsync());

			return new Movie {
				Id = id,
				PosterPath = movieDetails?.poster_path,
				Title = movieDetails?.title,
				ReleaseDate = movieDetails?.release_date
			};
		}));
		
		return Ok(new PollResponse() {
			Question = poll.Question,
			Votes = poll.Votes,
			Expiration = poll.Expiration,
			Movies = movies,
			ServerRestricted = poll.GuildId != null,
			CreatedAt = poll.CreatedAt,
		});
	}

	public class VoteDTO {
		public required string AccessToken { get; set; }
		public required string MovieId { get; set; }
	}

	[HttpPost("{id}/vote", Name = "Vote")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Vote([FromRoute] Guid id, [FromBody] VoteDTO body) {
		using var db = new LbPollContext();
		var discord = new DiscordOauth2(_settings);
		var client = new HttpClient();

		var user = await discord.GetUser(body.AccessToken);

		if (user == null) {
			return Unauthorized("Could not get your account.");
		}

		var poll = await db.FindAsync<Poll>(id);

		if (poll == null) {
			return NotFound("Could not find poll.");
		}

		if (!poll.MovieIds.Any((id) => id == body.MovieId)) {
			return BadRequest("This movie isn't an option.");
		}

		if (poll.Expiration != null && poll.Expiration < DateTime.Now) {
			return BadRequest("Poll has expired.");
		}

		if (poll.GuildId != null) {
			var guilds = await discord.GetGuilds(body.AccessToken);

			if (!guilds.Any(guild => guild.id == poll.GuildId)) {
				return BadRequest("This poll is restricted to a server that you aren't in.");
			}
		}

		try {
			poll.Votes.Add(new() {
				Poll = poll,
				MovieId = body.MovieId,
				PollId = poll.Id,
				UserId = user.id
			});
		} catch(DbUpdateException ex) {
			throw new Exception(ex.InnerException?.Message);
		}

		await db.SaveChangesAsync();

		return Ok();
	}
}
