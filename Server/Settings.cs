namespace Server;

public sealed class Settings
{
    public required DiscordSettings Discord { get; set; }
    public required string TmdbKey { get; set; }

    public class DiscordSettings
    {
        public required string RedirectUri { get; set; }
        public required string Secret { get; set; }
        public required string Id { get; set; }
    }
}
