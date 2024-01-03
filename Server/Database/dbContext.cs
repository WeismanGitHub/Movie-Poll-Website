using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

public class LbPollContext : DbContext {
    public DbSet<Poll> Polls { get; set; }

    public string DbPath { get; }

    public LbPollContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "lettreboxd-poll.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}

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
	public required string Question {  get; set; }
	[Required]
	public List<Vote> Votes { get; set; } = new();
	[Required]
	public string? ServerId { get; set; }
}
