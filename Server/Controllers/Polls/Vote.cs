using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("API/Polls")]
public class VoteController : ControllerBase
{
    public class VoteBody
    {
        public required string AccessToken { get; set; }
        public required string MovieId { get; set; }
    }

    private class Validator : AbstractValidator<VoteBody>
    {
        public Validator()
        {
            RuleFor(vote => vote.AccessToken)
                .NotEmpty()
                .NotNull()
                .MaximumLength(100)
                .WithMessage("Access Token must be between 1 and 100 characters.");

            RuleFor(vote => vote.MovieId)
                .NotEmpty()
                .NotNull()
                .MaximumLength(50)
                .WithMessage("Movie Id must be between 1 and 50 characters.");
        }
    }

    [HttpPost("{id}/vote", Name = "Vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Vote(
        [FromRoute] Guid id,
        [FromBody] VoteBody body,
        DiscordOauth2 discord,
        MoviePollsContext db
    )
    {
        ValidationResult validationResult = new Validator().Validate(body);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        DiscordOauth2.User user = await discord.GetUser(body.AccessToken);

        if (user == null)
        {
            return Unauthorized("Could not get your account.");
        }

        Poll? poll = await db.FindAsync<Poll>(id);

        if (poll == null)
        {
            return NotFound("Could not find poll.");
        }

        if (!poll.MovieIds.Any((id) => id == body.MovieId))
        {
            return BadRequest("This movie isn't an option.");
        }

        if (poll.Expiration != null && poll.Expiration < DateTime.Now)
        {
            return BadRequest("Poll has expired.");
        }

        if (poll.GuildId != null)
        {
            List<DiscordOauth2.Guild> guilds = await discord.GetGuilds(body.AccessToken);

            if (!guilds.Any(guild => guild.id == poll.GuildId))
            {
                return BadRequest("This poll is restricted to a server that you aren't in.");
            }
        }

        try
        {
            poll.Votes.Add(
                new()
                {
                    Poll = poll,
                    MovieId = body.MovieId,
                    PollId = poll.Id,
                    UserId = user.id
                }
            );
        }
        catch (DbUpdateException ex)
        {
            throw new Exception(ex.InnerException?.Message);
        }

        await db.SaveChangesAsync();

        return Ok();
    }
}
