using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Server;

public class DiscordOauth2
{
    private readonly Configuration _config;

    public DiscordOauth2(Configuration config)
    {
        _config = config;
    }

    public class Guild
    {
        public required string id { get; set; }
        public required string name { get; set; }
        public required string icon { get; set; }
        public required bool owner { get; set; }
        public required int permissions { get; set; }
        public required string permissions_new { get; set; }
        public required List<string> features { get; set; }
    }

    public async Task<string> GetAccessToken(string authCode)
    {
        var client = new HttpClient();

        var reqBody = new Dictionary<string, string>()
        {
            { "client_id", _config.Discord.Id },
            { "client_secret", _config.Discord.Secret },
            { "code", authCode },
            { "grant_type", "authorization_code" },
            { "redirect_uri", _config.Discord.RedirectUri },
            { "scope", "identify guilds" },
        };

        HttpResponseMessage res = await client.PostAsync(
            "https://discord.com/api/oauth2/token",
            new FormUrlEncodedContent(reqBody)
        );

        var content = await res.Content.ReadAsStringAsync();
        JsonElement json = JsonDocument.Parse(content).RootElement;

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception(json.GetString("error_description"));
        }

        var accessToken = json.GetString("access_token");

        if (accessToken == null)
        {
            throw new Exception("Could not read access token.");
        }

        return accessToken;
    }

    public async Task<List<Guild>> GetGuilds(string accessToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        HttpResponseMessage res = await client.GetAsync("https://discord.com/api/users/@me/guilds");
        var content = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
        {
            Console.WriteLine(content);
            throw new Exception(JsonDocument.Parse(content).RootElement.GetString("message"));
        }

        List<Guild>? guilds = JsonSerializer.Deserialize<List<Guild>>(content);

        if (guilds == null)
        {
            throw new Exception("Could not read guilds.");
        }

        return guilds;
    }

    public class User
    {
        public required string id { get; set; }
        public required string username { get; set; }
        public required string discriminator { get; set; }
        public required string? global_name { get; set; }
        public required string? avatar { get; set; }
        public bool bot { get; set; }
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

    public async Task<User> GetUser(string accessToken)
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );
        HttpResponseMessage res = await client.GetAsync("https://discord.com/api/users/@me");
        var content = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        User? user = JsonSerializer.Deserialize<User>(content);
        return user!;
    }
}
