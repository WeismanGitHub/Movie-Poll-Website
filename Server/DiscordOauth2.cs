using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Server;

public class DiscordOauth2 {
	private readonly Settings _settings;

	public DiscordOauth2(Settings settings) {
		_settings = settings;
	}

	public class Guild {
		public string id { get; set; }
		public string name { get; set; }
		public string icon { get; set; }
		public bool owner { get; set; }
		public int permissions { get; set; }
		public string permissions_new { get; set; }
		public List<string> features { get; set; }
	}

	public async Task<string> GetAccessToken(string authCode) {
		var client = new HttpClient();

		var reqBody = new Dictionary<string, string>() {
			{ "client_id", _settings.Discord.Id },
			{ "client_secret", _settings.Discord.Secret },
			{ "code", authCode },
			{ "grant_type", "authorization_code" },
			{ "redirect_uri", _settings.Discord.RedirectUri },
			{ "scope", "identify guilds" },
		};

		var res = await client.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(reqBody));
		
		var content = await res.Content.ReadAsStringAsync();
		var json = JsonDocument.Parse(content).RootElement;

		if (!res.IsSuccessStatusCode) {
			throw new Exception(json.GetString("error_description"));
		}

		return json.GetString("access_token");
	}

	public async Task<List<Guild>> GetGuilds(string accessToken) {
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		var res = await client.GetAsync("https://discord.com/api/users/@me/guilds");
		var content = await res.Content.ReadAsStringAsync();

		if (!res.IsSuccessStatusCode) {
			Console.WriteLine(content);
			throw new Exception(JsonDocument.Parse(content).RootElement.GetString("message"));
		}

		var guilds = JsonSerializer.Deserialize<List<Guild>>(content);
		return guilds!;
	}

	public class User {
		public required string id { get; set; }
		public required string username {  get; set; }
		public required string discriminator { get; set; }
		public required string? global_name { get; set; }
		public required string? avatar {  get; set; }
		public bool bot {  get; set; }
		public bool system { get; set; }
		public bool mfa_enabled { get; set; }
		public string? banner { get; set; }
		public int? accent_color { get; set; }
		public string? locale { get; set; }
		public int? flags { get; set; }
		public int? premium_type { get; set; }
		public int? public_flags { get; set; }
		public string? avatar_decoration { get; set; }
}

	public async Task<User> GetUser(string accessToken) {
		var client = new HttpClient();

		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		var res = await client.GetAsync("https://discord.com/api/users/@me");
		var content = await res.Content.ReadAsStringAsync();

		if (!res.IsSuccessStatusCode) {
			throw new Exception(content);
		}

		var user = JsonSerializer.Deserialize<User>(content);
		return user!;
	}
}
