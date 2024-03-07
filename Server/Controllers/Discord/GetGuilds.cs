using System.ComponentModel.DataAnnotations;

namespace Server.Controllers;

[ApiController]
[Route("API/Discord")]
public class GetGuildsController : ControllerBase
{
    [HttpGet("Guilds", Name = "GetGuilds")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGuilds(
        [FromQuery(Name = "token"), Required, MaxLength(50)] string accessToken,
        Settings settings
    )
    {
        var discord = new DiscordOauth2(settings);
        List<DiscordOauth2.Guild> guilds = await discord.GetGuilds(accessToken);

        return Ok(guilds);
    }
}
