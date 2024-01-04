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
	public Guid Id { get; } = Guid.NewGuid();
	[Required, MaxLength(500), MinLength(1)]
	public required string Question { get; set; }
	[Required]
	public List<Vote> Votes { get; } = new();
	[Required]
	public string? ServerId { get; set; }
	public DateTime CreatedAt { get; } = DateTime.Now;
	public DateTime? Expiration { get; set; }
}
