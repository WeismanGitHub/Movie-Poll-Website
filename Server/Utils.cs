using API;

namespace server;

public class Utils {
	private readonly Settings _settings;

	public Utils(Settings settings) {
		_settings = settings;
	}

	public class Guild {
		public int Id;
	}
	public async Task<List<Guild>> GetGuilds(string authCode) {
		Console.WriteLine(authCode, _settings);
		//	const tokenResponseData = await request('https://discord.com/api/oauth2/token', {
		//	method: 'POST',
		//	body: new URLSearchParams({
		//		client_id: process.env.CLIENT_ID,
		//		client_secret: process.env.CLIENT_SECRET,
		//		code,
		//		grant_type: 'authorization_code',
		//		redirect_uri: process.env.REDIRECT_URI,
		//		scope: 'guilds'
		//	}).toString(),
		//	headers:
		//		{
		//			'Content-Type': 'application/x-www-form-urlencoded',
		//	},
		//});

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
}
