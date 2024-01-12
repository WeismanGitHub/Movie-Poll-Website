using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;
using API;

namespace server;

public class Utils {
	private readonly Settings _settings;

	public Utils(Settings settings) {
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

	public async Task<List<Guild>> GetGuilds(string authCode) {
		var client = new HttpClient();
		
		var reqBody = new Dictionary<string, string>() {
			{ "client_id", _settings.Discord.Id },
			{ "client_secret", _settings.Discord.Secret },
			{ "code", authCode },
			{ "grant_type", "authorization_code" },
			{ "redirect_uri", _settings.Discord.RedirectUri },
			{ "scope", "identify guilds" },
		};
		
		var oauthRes = await client.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(reqBody));
		var content = await oauthRes.Content.ReadAsStringAsync();
		var accessToken = JsonDocument.Parse(content).RootElement.GetString("access_token");

		if (!oauthRes.IsSuccessStatusCode) {
			throw new Exception("Fetching access token failed.");
		}

		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		var guildsRes = await client.GetAsync("https://discord.com/api/users/@me/guilds");

		if (!guildsRes.IsSuccessStatusCode) {
			throw new Exception("Fetching guilds failed.");
		}

		var guilds = JsonSerializer.Deserialize<List<Guild>>(await guildsRes.Content.ReadAsStringAsync());
		return guilds!;
	}

	public class User {
		public string Id { get; set; }
	}

	public async Task<User> GetUser(string authCode) {
		Console.WriteLine(authCode);
		return new();
	}
}
