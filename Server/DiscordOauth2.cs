using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web.Http;
using API;

namespace server;

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
		public string Id { get; set; }
		// there are more properties, but i dont need them
	}

	public async Task<User> GetUser(string accessToken) {
		var client = new HttpClient();

		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		var res = await client.GetAsync("https://discord.com/api/users/@me/");
		var content = await res.Content.ReadAsStringAsync();

		if (!res.IsSuccessStatusCode) {
			throw new Exception(JsonDocument.Parse(content).RootElement.GetString("error_description"));
		}

		var user = JsonSerializer.Deserialize<User>(content);
		return user!;
	}
}
