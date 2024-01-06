using Microsoft.AspNetCore.Mvc;
using Database;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsController : ControllerBase {
	public class CreatePollDTO {
		public required string Question { get; set; }
		public DateTime? Expiration { get; set; }
		public string? ServerId { get; set; }
		public string? AuthCode { get; set; }
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

		input.ItemIds.ForEach(itemId => {
			Console.WriteLine(itemId);
			// check that they're valid ids
		});

		if (input.ServerId != null && input.AuthCode == null) {
			return BadRequest("You must be logged in to restrict a poll to a server.");
		} else if (input.ServerId != null && input.AuthCode != null) {

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
}
