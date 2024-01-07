namespace API;

public sealed class Settings {
	public DiscordSettings Discord { get; set; }
	public string TmdbKey { get; set; }
	public class DiscordSettings {
		public string RedirectUri { get; set; }
		public string Secret { get; set; }
		public string Id { get; set; }
	}
}
