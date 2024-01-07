using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Database;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsController : ControllerBase {
	public class CreatePollDTO {
		[Required, MaxLength(500), MinLength(1)]
		public required string Question { get; set; }
		public DateTime? Expiration { get; set; }
		public string? ServerId { get; set; }
		public string? AuthCode { get; set; }
		[Required, MinLength(2), MaxLength(50)]
		public required List<string> ItemIds {  get; set; }
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
		} else if (input.ItemIds.Count < 2 || input.ItemIds.Count > 50) {
			return BadRequest("Selected items must be between 2 and 50.");
		}

		if (input.ItemIds.Any(itemId => itemId.Length > 30)) {
			return BadRequest("Invalid Ids.");
		}

		if (input.ServerId != null && input.AuthCode == null) {
			return BadRequest("You must be logged in to restrict a poll to a server.");
		} else if (input.ServerId != null && input.AuthCode != null) {
			// do the auth process
		}

		var poll = new Poll {
			Question = input.Question,
			Expiration = input.Expiration,
			ServerId = input.ServerId,
			ItemIds = input.ItemIds,
		};

		db.Add(poll);
		await db.SaveChangesAsync();

		return Created($"/polls/{poll.Id}", poll.Id);
    }

	public class PollResponse {
		public required string Question { get; set; }
		public required IEnumerable<string> Votes {  get; set; }
		public required List<string> ItemIds {  get; set; }
		public required DateTime CreatedAt { get; set; }
		public required bool ServerRestricted { get; set; }
		public DateTime? Expiration { get; set; }
	}

	[HttpGet(Name = "GetPoll")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
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
			Votes = poll.Votes.Select(vote => vote.ItemId),
			Expiration = poll.Expiration,
			ItemIds = poll.ItemIds,
			ServerRestricted = poll.ServerId != null,
			CreatedAt = poll.CreatedAt,
		});
	}

	[HttpPost(Name = "Vote")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Vote(Guid id) {
		using var db = new LbPollContext();
		var client = new HttpClient();

		var poll = await db.FindAsync<Poll>(id);

		if (poll == null) {
			return NotFound("Could not find poll.");
		}

		if (poll.Expiration != null && poll.Expiration < DateTime.Now) {
			return BadRequest("Poll has expired");
		}

		if (poll.ServerId != null) {
			var authCode = Request.Headers.Authorization;
			Console.WriteLine(authCode);
		}

		return Ok();
	}
}
