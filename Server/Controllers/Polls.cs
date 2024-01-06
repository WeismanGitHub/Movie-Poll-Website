using Microsoft.AspNetCore.Mvc;
using Database;

namespace Movie_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsController : ControllerBase {
	public class CreatePollDTO {
		public required string Question { get; set; }
		public DateTime? Expiration { get; set; }
	}

	[HttpPost(Name = "CreatePoll")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreatePoll(CreatePollDTO input) {
		using var db = new LbPollContext();
		var client = new HttpClient();

		if (input.Expiration > DateTime.Now.AddMonths(6) || input.Expiration <= DateTime.Now) {
			return BadRequest("Expiration must be greater than the current time and less than 6 months.");
		}

		//if () {

		//}

		var poll = new Poll {
			Question = input.Question,
			Expiration = input.Expiration,
		};

		db.Add(poll);
		await db.SaveChangesAsync();

		return Created(string.Empty, poll.Id);
    }
}
