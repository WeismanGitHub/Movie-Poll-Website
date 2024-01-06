using Microsoft.EntityFrameworkCore;

namespace Database;
public class LbPollContext : DbContext {
    public DbSet<Poll> Polls { get; set; }

    public string DbPath { get; }

    public LbPollContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "movie-poll.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}
