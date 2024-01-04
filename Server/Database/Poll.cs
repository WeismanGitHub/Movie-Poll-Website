using System.ComponentModel.DataAnnotations;

namespace Database;

public class Vote {
	[Key, Required]
	public required string UserId { get; set; }
	[Required]
	public string ChoiceId { get; set; }
}

public class Poll {
	[Key, Required]
	public Guid Id { get; set; } = Guid.NewGuid();
	[Required]
	public required string Question { get; set; }
	[Required]
	public List<Vote> Votes { get; set; } = new();
	[Required]
	public string? ServerId { get; set; }
}
