using Microsoft.AspNetCore.Mvc;
using Database;

namespace Letterboxd_Poll_Website.Server.Controllers;

[ApiController]
[Route("api/polls")]
public class PollsController : ControllerBase {
	public class CreatePollDTO {
		public string Question { get; set; }
		public DateTime? Expiration { get; set; }
	}

	[HttpPost(Name = "CreatePoll")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult CreatePoll(CreatePollDTO input) {
		using var db = new LbPollContext();

		var poll = new Poll {
			Question = input.Question,
			Expiration = input.Expiration,
		};

		db.Add(poll);
		db.SaveChanges();

		return Created(string.Empty, poll.Id);
    }
}
