using System.ComponentModel.DataAnnotations;

namespace Server.Controllers;

[ApiController]
[Route("API/Discord")]
public class GetGuildsController : ControllerBase
{
    [HttpGet("Guilds", Name = "GetGuilds")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces<List<DiscordOauth2.Guild>>]
    [Tags("Discord")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGuilds(
        [FromQuery(Name = "token"), Required, MaxLength(50)] string accessToken,
        DiscordOauth2 discord
    )
    {
        List<DiscordOauth2.Guild> guilds = await discord.GetGuilds(accessToken);

        return Ok(guilds);
    }
}
