using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Database;

public class Vote {
	[Key, Required]
	public Poll Poll { get; set; }
	[Key, Required]
	public required string UserId { get; set; }
	[Required]
	public string MovieId { get; set; }
}

public class Poll {
	[Key, Required]
	public Guid Id { get; set; } = Guid.NewGuid();
	[Required, MaxLength(500), MinLength(1)]
	public required string Question { get; set; }
	[Required]
	public List<Vote> Votes { get; set; } = new();
	public string? GuildId { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? Expiration { get; set; }
	[Required, MinLength(2), MaxLength(50)]
	public List<string> MovieIds { get; set; } = new();
}
