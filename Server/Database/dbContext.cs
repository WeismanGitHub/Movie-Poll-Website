using Microsoft.EntityFrameworkCore;

namespace Database;
public class LbPollContext : DbContext {
    public DbSet<Poll> Polls { get; set; }

    public string DbPath { get; }

    public LbPollContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "movie-polls.db");
    }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<Poll>()
			.HasMany(p => p.Votes)
			.WithOne(v => v.Poll);

		modelBuilder.Entity<Vote>()
			.HasKey(v => new { v.PollId, v.UserId });
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}
