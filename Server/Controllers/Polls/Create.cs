using FluentValidation.Results;

namespace Server.Controllers;

[ApiController]
[Route("API/Polls")]
public class CreateController : ControllerBase {
	public class CreatePollBody {
		public required string Question { get; set; }
		public DateTime? Expiration { get; set; }
		public string? GuildId { get; set; }
		public string? AccessToken { get; set; }
		public required List<string> MovieIds { get; set; }
	}

	private class Validator : AbstractValidator<CreatePollBody> {
		public Validator() {
			RuleFor(poll => poll.Question)
				.NotEmpty()
				.NotNull()
				.MinimumLength(1)
				.MaximumLength(500)
				.WithMessage("Question must be between 1 and 500 characters.");

			RuleFor(poll => poll.Expiration)
				.Must(expiration => expiration > DateTime.Now)
				.When(poll => poll.Expiration != null)
				.WithMessage("Expiration must be in the future.");

			RuleFor(poll => poll.GuildId)
				.Must(guildId => guildId != null)
				.When(poll => poll.AccessToken != null)
				.WithMessage("Guild Id is required.");

			RuleFor(poll => poll.AccessToken)
				.Must(accessToken => accessToken != null)
				.When(poll => poll.GuildId != null)
				.WithMessage("Access Token is required.");

			RuleFor(poll => poll.MovieIds)
				.NotNull()
				.NotEmpty()
				.WithMessage("MovieIds is required.")
				.Must(movieIds => movieIds.Count >= 2)
				.WithMessage("MovieIds must contain at least 2 ids.")
				.Must(movieIds => movieIds.Count <= 50)
				.WithMessage("MovieIds must contain at most 50 ids.")
				.Must(movieIds => movieIds.Count == movieIds.Distinct().Count())
				.WithMessage("MovieIds must contain unique values.");

			RuleForEach(poll => poll.MovieIds)
				.NotNull()
				.NotEmpty()
				.MaximumLength(50)
				.WithMessage("Invalid Movie Id(s).");
		}
	}

	[HttpPost(Name = "CreatePoll")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreatePoll([FromBody] CreatePollBody body, Settings settings) {
		ValidationResult validationResult = new Validator().Validate(body);

		if (!validationResult.IsValid) {
			return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
		}

		if (body.GuildId != null && body.AccessToken != null) {
			var discord = new DiscordOauth2(settings);
			List<DiscordOauth2.Guild> guilds = await discord.GetGuilds(body.AccessToken);

			if (!guilds.Any(guild => guild.id == body.GuildId)) {
				return Forbid("You must be in the server to restrict this poll.");
			}
		}

		using var db = new LbPollContext();
		var client = new HttpClient();

		var poll = new Poll {
			Question = body.Question,
			Expiration = body.Expiration,
			GuildId = body.GuildId,
			MovieIds = body.MovieIds,
		};

		db.Add(poll);
		await db.SaveChangesAsync();

		return Created($"/polls/{poll.Id}", poll.Id);
	}
}
