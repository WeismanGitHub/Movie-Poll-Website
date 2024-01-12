using API;

namespace server;

public class Utils {
	private readonly Settings _settings;

	public Utils(Settings settings) {
		_settings = settings;
	}

	public class Guild {
		public string Id;
	}

	public async Task<List<Guild>> GetGuilds(string authCode) {
		var client = new HttpClient();
		
		var content = new Dictionary<string, string>() {
			{ "client_id", _settings.Discord.Id },
			{ "client_secret", _settings.Discord.Secret },
			{ "code", authCode },
			{ "grant_type", "authorization_code" },
			{ "code", authCode },
			{ "redirect_uri", _settings.Discord.RedirectUri },
			{ "scope", "identify guilds" },
		};

		var res = await client.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(content));
		Console.WriteLine(res.StatusCode);

		//	const oauthData = await tokenResponseData.body.json();
		//const AccessToken = oauthData.access_token;
		//request("GET", "/users/@me/guilds", undefined, {
		//auth:
		//	{
		//	type: "Bearer",
		//		creds: AccessToken,
		//	},
		//});

		return new();
	}

	public class User {
		public string Id { get; set; }
	}

	public async Task<User> GetUser(string authCode) {
		Console.WriteLine(authCode);
		return new();
	}
}
