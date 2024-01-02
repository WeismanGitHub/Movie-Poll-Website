using Microsoft.EntityFrameworkCore;

public class LbPollContext : DbContext {
    public DbSet<Poll> Polls { get; set; }

    public string DbPath { get; }

    public LbPollContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "lettreboxd-poll.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}

public class Vote {
    public string userId { get; set; }
    public string choiceId { get; set; }
}

public class Poll {
    public string Id { get; set; }
    public List<Vote> Votes { get; } = new();
}
