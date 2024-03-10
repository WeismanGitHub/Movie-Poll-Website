namespace Server;

public sealed class Configuration
{
    public required DiscordConfig Discord { get; set; }
    public required string TmdbKey { get; set; }

    public class DiscordConfig
    {
        public required string RedirectUri { get; set; }
        public required string Secret { get; set; }
        public required string Id { get; set; }
    }
}
